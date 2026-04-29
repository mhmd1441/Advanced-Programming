using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TaskManagementMvc.Services;
using TaskManagementMvc.ViewModels;

namespace TaskManagementMvc.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IApiClient _apiClient;

        public CategoriesController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            ViewBag.UserRole = GetCurrentRole();

            try
            {
                var categories = await _apiClient.GetAsync<List<CategoryViewModel>>("/api/Categories");
                return View(categories ?? new List<CategoryViewModel>());
            }
            catch (ApiRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<CategoryViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() != "Admin")
                return RedirectToAction(nameof(Index));

            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() != "Admin")
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _apiClient.PostAsync<CategoryFormViewModel, CategoryViewModel>("/api/Categories", model);
                TempData["Success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() != "Admin")
                return RedirectToAction(nameof(Index));

            var category = await _apiClient.GetAsync<CategoryViewModel>($"/api/Categories/{id}");
            if (category == null)
                return NotFound();

            return View(new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() != "Admin")
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _apiClient.PutAsync<CategoryFormViewModel, CategoryViewModel>($"/api/Categories/{id}", model);
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() != "Admin")
                return RedirectToAction(nameof(Index));

            try
            {
                await _apiClient.DeleteAsync($"/api/Categories/{id}");
                TempData["Success"] = "Category deleted successfully.";
            }
            catch (ApiRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JWToken"));
        }

        private string GetCurrentRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? "Employee";
        }
    }
}
