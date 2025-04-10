using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoList.Models;
using Microsoft.Extensions.Options;

namespace ToDoList.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        public BookController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings>apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = apiSettings.Value.BaseUrl;
        }
        private HttpClient GetClient()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri($"{_baseUrl}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
        // GET: Book
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var response = await client.GetAsync("api/BooksAPI");
            var books = await response.Content.ReadFromJsonAsync<List<Book>>();
            return View(books);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            var client = GetClient();
            var response = await client.PostAsJsonAsync("api/BooksAPI", book);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Failed to create book. API says : {errorContent}");
            return View(book);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/BooksAPI/{id}");
            //
            if (!response.IsSuccessStatusCode)
            {
                // Handle not found or error
                return NotFound();
            }
            //
            var book = await response.Content.ReadFromJsonAsync<Book>();

            //
            var userResponse = await client.GetAsync("api/UsersAPI");
            List<User> users = new();
            if (userResponse.IsSuccessStatusCode)
            {
                users = await userResponse.Content.ReadFromJsonAsync<List<User>>();
            }
            else
            {
                var error = await userResponse.Content.ReadAsStringAsync();
                Console.WriteLine("User API failed: " + error);
            }
            //var users = await userResponse.Content.ReadFromJsonAsync<List<User>>();
            ViewBag.UserId = new SelectList(users, "Id", "Name", book.UserId);
            //
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Book book)
        {
            var client = GetClient();

            if (book.UserId == 0)
            {
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userIdStr))
                {
                    book.UserId = int.Parse(userIdStr);
                }
            }

            var response = await client.PutAsJsonAsync($"api/BooksAPI/{book.BookId}", book);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Failed to update book. API says: " + error);
                return View(book);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/BooksAPI/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var book = await response.Content.ReadFromJsonAsync<Book>();
            return View(book);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"api/BooksAPI/{id}");
            return RedirectToAction(nameof (Index));
        }
    }
}
