using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementMvc.Services;
using TaskManagementMvc.ViewModels;

namespace TaskManagementMvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly IApiClient _apiClient;

        public UsersController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() != "Admin")
            {
                ViewBag.Error = "Only admins can access user management.";
                return View(new List<UserManagementViewModel>());
            }

            try
            {
                var users = await _apiClient.GetAsync<List<UserManagementViewModel>>("/api/Users");
                return View(users ?? new List<UserManagementViewModel>());
            }
            catch (ApiRequestException ex)
            {
                ViewBag.Error = ex.StatusCode == 403
                    ? "Only admins can access user management."
                    : ex.Message;

                return View(new List<UserManagementViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string id, UpdateUserRoleViewModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() != "Admin")
            {
                TempData["Error"] = "Only admins can change user roles.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid role selected.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _apiClient.PutAsync<UpdateUserRoleViewModel, string>($"/api/Users/{id}/role", model);
                TempData["Success"] = "User role updated successfully.";
            }
            catch (ApiRequestException ex)
            {
                TempData["Error"] = ex.StatusCode == 403
                    ? "Only admins can change user roles."
                    : ex.Message;
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
