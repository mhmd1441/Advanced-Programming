using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetAllAsync(string currentUserId, bool canViewAllTasks)
        {
            var query = _context.TaskItems
                .Include(t => t.Category)
                .Include(t => t.AssignedToUser)
                .AsQueryable();

            if (!canViewAllTasks)
                query = query.Where(t => t.AssignedToUserId == currentUserId);

            var tasks = await query
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
                        t.Category!.Id,
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

            return tasks;
        }

        public async Task<object?> GetByIdAsync(int id, string currentUserId, bool canViewAllTasks)
        {
            var query = _context.TaskItems
                .Include(t => t.Category)
                .Include(t => t.AssignedToUser)
                .Where(t => t.Id == id);

            if (!canViewAllTasks)
                query = query.Where(t => t.AssignedToUserId == currentUserId);

            return await query
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
                        t.Category!.Id,
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
        }

        public async Task<object?> CreateAsync(TaskCreateDto dto, string currentUserId, bool canAssignToOthers)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
                return null;

            var assignedUserId = canAssignToOthers ? dto.AssignedToUserId : currentUserId;

            if (assignedUserId != null)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == assignedUserId);

                if (!userExists)
                    return null;
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
                AssignedToUserId = assignedUserId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<object?> UpdateAssignmentAsync(int id, TaskAssignmentDto dto)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return null;

            if (dto.AssignedToUserId != null)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.AssignedToUserId);

                if (!userExists)
                    return null;
            }

            task.AssignedToUserId = dto.AssignedToUserId;
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<object?> UpdateAsync(int id, TaskCreateDto dto)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return null;

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
                return null;

            if (dto.AssignedToUserId != null)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.AssignedToUserId);

                if (!userExists)
                    return null;
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate.ToUniversalTime();
            task.CategoryId = dto.CategoryId;
            task.AssignedToUserId = dto.AssignedToUserId;

            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return false;

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
