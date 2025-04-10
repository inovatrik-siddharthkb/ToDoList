using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register() => View();
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:44316/api/Users/register", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var errorContent = await response.Content.ReadAsStringAsync();

            if (errorContent.Contains("Email"))
                ModelState.AddModelError(nameof(dto.Email), "Email already exists.");
            else if (errorContent.Contains("Username"))
                ModelState.AddModelError(nameof(dto.Username), "Username already exists.");
            else
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");

            return View(dto);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:44316/api/Users/login", dto);

            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                HttpContext.Session.SetString("JWTToken", result.Token);
                return RedirectToAction("Index", "Book");
            }

            ViewBag.Error = "Invalid login.";
            return View();

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            return RedirectToAction("Login");
        }
    }
}
