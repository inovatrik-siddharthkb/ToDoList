using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class PageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public PageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        private HttpClient GetClient()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44316/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // GET: Page
        public async Task<IActionResult> Index(int bookId)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/PagesAPI?bookId={bookId}");
            var pages = await response.Content.ReadFromJsonAsync<List<Page>>();
            ViewBag.BookId = bookId;
            return View(pages);
        }

        // GET: Page/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/PagesAPI/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var page = await response.Content.ReadFromJsonAsync<Page>();
            return View(page);
        }

        // GET: Page/Create
        public IActionResult Create(int bookId)
        {
            var page = new Page { PbookId = bookId };
            return View(page);
        }

        // POST: Page/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (!ModelState.IsValid)
                return View(page);

            var client = GetClient();

            if (page.PbookId == 0)
            {
                ModelState.AddModelError("", "BookId is required.");
                return View(page);
            }

            var response = await client.PostAsJsonAsync("api/PagesAPI", page);
            if (response.IsSuccessStatusCode)
            {
                    return RedirectToAction("Index", new { bookId = page.PbookId });
            }

            var error = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError("", "Failed to create page. API says: " + error);

            return View(page);
        }

        // GET: Page/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/PagesAPI/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var page = await response.Content.ReadFromJsonAsync<Page>();
            return View(page);
        }

        // POST: Page/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Page page)
        {
            if (id != page.PageId)
                return NotFound();

            if (page.PbookId == 0)
            {
                ModelState.AddModelError("PbookId", "BookId is required.");
                return View(page);
            }

            var client = GetClient();
            var response = await client.PutAsJsonAsync($"api/PagesAPI/{id}", page);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", new { bookId = page.PbookId });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"API Error: {errorContent}");

            return View(page);
        }

        // GET: Page/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"api/PagesAPI/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var page = await response.Content.ReadFromJsonAsync<Page>();
            return View(page);
        }

        // POST: Page/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClient();

            // Fetch to get book ID for redirect
            var pageResponse = await client.GetAsync($"api/PagesAPI/{id}");
            var page = await pageResponse.Content.ReadFromJsonAsync<Page>();

            var response = await client.DeleteAsync($"api/PagesAPI/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index), new { bookId = page.PbookId });

            ModelState.AddModelError("", "Failed to delete page");
            return View(page);
        }
    }
}
