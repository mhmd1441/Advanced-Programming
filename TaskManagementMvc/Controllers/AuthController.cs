using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using TaskManagementMvc.ViewModels;

namespace TaskManagementMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiSettings:BaseUrl"];

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/api/Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password";
                return View(model);
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseBody);
            var token = document.RootElement.GetProperty("token").GetString();

            HttpContext.Session.SetString("JWToken", token!);
            HttpContext.Session.SetString("UserRole", GetRoleFromToken(token!));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiSettings:BaseUrl"];

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/api/Auth/register", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }

            TempData["Success"] = "Account created successfully. You can log in now.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            HttpContext.Session.Remove("UserRole");
            return RedirectToAction("Login");
        }

        private static string GetRoleFromToken(string token)
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
                return "Employee";

            var payload = parts[1]
                .Replace('-', '+')
                .Replace('_', '/');

            payload = payload.PadRight(payload.Length + ((4 - payload.Length % 4) % 4), '=');

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            using var document = JsonDocument.Parse(json);

            foreach (var claim in document.RootElement.EnumerateObject())
            {
                if (claim.Name == "role" ||
                    claim.Name.EndsWith("/role", StringComparison.OrdinalIgnoreCase))
                {
                    return claim.Value.ValueKind == JsonValueKind.Array
                        ? claim.Value.EnumerateArray().FirstOrDefault().GetString() ?? "Employee"
                        : claim.Value.GetString() ?? "Employee";
                }
            }

            return "Employee";
        }
    }
}
