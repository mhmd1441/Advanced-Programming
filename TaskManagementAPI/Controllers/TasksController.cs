using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _context.TaskItems
                .Include(t => t.Category)
                .Include(t => t.AssignedToUser)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.Priority,
                    t.DueDate,
                    t.CreatedAt,
                    Category = new
                    {
                        t.Category.Id,
                        t.Category.Name
                    },
                    AssignedToUser = t.AssignedToUser == null ? null : new
                    {
                        t.AssignedToUser.Id,
                        t.AssignedToUser.FirstName,
                        t.AssignedToUser.LastName,
                        t.AssignedToUser.Email
                    }
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _context.TaskItems
                .Include(t => t.Category)
                .Include(t => t.AssignedToUser)
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.Priority,
                    t.DueDate,
                    t.CreatedAt,
                    Category = new
                    {
                        t.Category.Id,
                        t.Category.Name
                    },
                    AssignedToUser = t.AssignedToUser == null ? null : new
                    {
                        t.AssignedToUser.Id,
                        t.AssignedToUser.FirstName,
                        t.AssignedToUser.LastName,
                        t.AssignedToUser.Email
                    }
                })
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound("Task not found");

            return Ok(task);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(TaskCreateDto dto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
                return BadRequest("Category does not exist");

            if (dto.AssignedToUserId != null)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.AssignedToUserId);

                if (!userExists)
                    return BadRequest("Assigned user does not exist");
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                Priority = dto.Priority,
                DueDate = dto.DueDate.ToUniversalTime(),
                CreatedAt = DateTime.UtcNow,
                CategoryId = dto.CategoryId,
                AssignedToUserId = dto.AssignedToUserId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskCreateDto dto)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return NotFound("Task not found");

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate.ToUniversalTime();
            task.CategoryId = dto.CategoryId;
            task.AssignedToUserId = dto.AssignedToUserId;

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return NotFound("Task not found");

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return Ok("Task deleted successfully");
        }
    }
}