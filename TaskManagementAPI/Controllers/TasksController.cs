using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskService.GetAllAsync(GetCurrentUserId(), CanViewAllTasks());
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _taskService.GetByIdAsync(id, GetCurrentUserId(), CanViewAllTasks());

            if (task == null)
                return NotFound("Task not found");

            return Ok(task);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> Create(TaskCreateDto dto)
        {
            var task = await _taskService.CreateAsync(dto, GetCurrentUserId(), User.IsInRole("Admin"));

            if (task == null)
                return BadRequest("Category or assigned user does not exist");

            return Ok(task);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskCreateDto dto)
        {
            var task = await _taskService.UpdateAsync(id, dto);

            if (task == null)
                return NotFound("Task not found");

            return Ok(task);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}/assignment")]
        public async Task<IActionResult> UpdateAssignment(int id, TaskAssignmentDto dto)
        {
            var task = await _taskService.UpdateAssignmentAsync(id, dto);

            if (task == null)
                return NotFound("Task or assigned user not found");

            return Ok(task);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _taskService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Task not found");

            return Ok("Task deleted successfully");
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        private bool CanViewAllTasks()
        {
            return User.IsInRole("Admin") || User.IsInRole("Manager");
        }
    }
}
