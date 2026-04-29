using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementMvc.Services;
using TaskManagementMvc.ViewModels;

namespace TaskManagementMvc.Controllers
{
    public class TasksController : Controller
    {
        private readonly IApiClient _apiClient;

        public TasksController(IApiClient apiClient)
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
                var tasks = await _apiClient.GetAsync<List<TaskItemViewModel>>("/api/Tasks");
                return View(tasks ?? new List<TaskItemViewModel>());
            }
            catch (ApiRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<TaskItemViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var task = await _apiClient.GetAsync<TaskItemViewModel>($"/api/Tasks/{id}");
            if (task == null)
                return NotFound();

            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() == "Manager")
            {
                TempData["Error"] = "Managers can reassign tasks but cannot create new tasks.";
                return RedirectToAction(nameof(Index));
            }

            var model = new TaskFormViewModel();
            await PopulateSelectLists(model);
            ViewBag.UserRole = GetCurrentRole();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskFormViewModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() == "Manager")
            {
                TempData["Error"] = "Managers can reassign tasks but cannot create new tasks.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await PopulateSelectLists(model);
                ViewBag.UserRole = GetCurrentRole();
                return View(model);
            }

            try
            {
                await _apiClient.PostAsync<TaskFormViewModel, TaskItemViewModel>("/api/Tasks", model);
                TempData["Success"] = "Task created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ApiRequestException ex)
            {
                ViewBag.Error = ex.Message;
                await PopulateSelectLists(model);
                ViewBag.UserRole = GetCurrentRole();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            if (GetCurrentRole() == "Employee")
            {
                TempData["Error"] = "Employees can view their tasks but cannot edit them.";
                return RedirectToAction(nameof(Index));
            }

            var task = await _apiClient.GetAsync<TaskItemViewModel>($"/api/Tasks/{id}");
            if (task == null)
                return NotFound();

            var model = new TaskFormViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CategoryId = task.Category?.Id ?? 0,
                AssignedToUserId = task.AssignedToUser?.Id
            };

            await PopulateSelectLists(model);
            ViewBag.UserRole = GetCurrentRole();
            ViewBag.IsAssignmentOnly = GetCurrentRole() == "Manager";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TaskFormViewModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var role = GetCurrentRole();

            if (role == "Employee")
            {
                TempData["Error"] = "Employees can view their tasks but cannot edit them.";
                return RedirectToAction(nameof(Index));
            }

            if (role != "Manager" && !ModelState.IsValid)
            {
                await PopulateSelectLists(model);
                ViewBag.UserRole = role;
                ViewBag.IsAssignmentOnly = role == "Manager";
                return View(model);
            }

            try
            {
                if (role == "Manager")
                {
                    await _apiClient.PutAsync<TaskAssignmentViewModel, TaskItemViewModel>(
                        $"/api/Tasks/{id}/assignment",
                        new TaskAssignmentViewModel { AssignedToUserId = model.AssignedToUserId });
                    TempData["Success"] = "Task assignment updated successfully.";
                }
                else
                {
                    await _apiClient.PutAsync<TaskFormViewModel, TaskItemViewModel>($"/api/Tasks/{id}", model);
                    TempData["Success"] = "Task updated successfully.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (ApiRequestException ex)
            {
                ViewBag.Error = ex.Message;
                await PopulateSelectLists(model);
                ViewBag.UserRole = role;
                ViewBag.IsAssignmentOnly = role == "Manager";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _apiClient.DeleteAsync($"/api/Tasks/{id}");
                TempData["Success"] = "Task deleted successfully.";
            }
            catch (ApiRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateSelectLists(TaskFormViewModel model)
        {
            var role = GetCurrentRole();
            var categories = await _apiClient.GetAsync<List<CategoryViewModel>>("/api/Categories") ?? new();

            model.Categories = categories
                .Select(category => new SelectListItem(category.Name, category.Id.ToString(), category.Id == model.CategoryId))
                .ToList();

            if (role == "Admin" || role == "Manager")
            {
                var users = await _apiClient.GetAsync<List<UserSummaryViewModel>>("/api/Users") ?? new();

                model.Users = users
                    .Select(user => new SelectListItem($"{user.FirstName} {user.LastName} - {user.JobTitle}", user.Id, user.Id == model.AssignedToUserId))
                    .ToList();
            }

            model.Users.Insert(0, new SelectListItem("Unassigned", string.Empty, string.IsNullOrWhiteSpace(model.AssignedToUserId)));
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
