using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(CategoryCreateDto dto);
        Task<Category?> UpdateAsync(int id, CategoryCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}