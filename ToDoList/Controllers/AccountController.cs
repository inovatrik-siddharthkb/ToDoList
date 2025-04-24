using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        public AccountController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings>apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = apiSettings.Value.BaseUrl;
        }

        [HttpGet]
        public IActionResult Register() => View();
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_baseUrl}api/Users/register", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var errorContent = await response.Content.ReadAsStringAsync();

            if (errorContent.Contains("Email"))
                ModelState.AddModelError(nameof(dto.Email), "Email already exists.");
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
            var response = await client.PostAsJsonAsync($"{_baseUrl}api/Users/login", dto);

            if (response.IsSuccessStatusCode)
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
