using System.ComponentModel.DataAnnotations;

namespace TaskManagementMvc.ViewModels
{
    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";
    }

    public class UpdateUserRoleViewModel
    {
        [Required]
        public string Role { get; set; } = "Employee";
    }
}
