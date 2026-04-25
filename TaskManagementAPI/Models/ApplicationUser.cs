using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string JobTitle { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<TaskItem> AssignedTasks { get; set; }
    }
}