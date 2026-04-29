using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<TaskItem> AssignedTasks { get; set; } = new();
    }
}
