using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services
{
    public interface ITaskService
    {
        Task<object> GetAllAsync(string currentUserId, bool canViewAllTasks);
        Task<object?> GetByIdAsync(int id, string currentUserId, bool canViewAllTasks);
        Task<object?> CreateAsync(TaskCreateDto dto, string currentUserId, bool canAssignToOthers);
        Task<object?> UpdateAsync(int id, TaskCreateDto dto);
        Task<object?> UpdateAssignmentAsync(int id, TaskAssignmentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
