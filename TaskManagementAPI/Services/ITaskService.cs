using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services
{
    public interface ITaskService
    {
        Task<object> GetAllAsync();
        Task<object?> GetByIdAsync(int id);
        Task<object?> CreateAsync(TaskCreateDto dto);
        Task<object?> UpdateAsync(int id, TaskCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}