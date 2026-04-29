using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskManagementMvc.ViewModels
{
    public class TaskItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public CategorySummaryViewModel? Category { get; set; }
        public UserSummaryViewModel? AssignedToUser { get; set; }
    }

    public class TaskFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "To Do";

        [Required]
        public string Priority { get; set; } = "Medium";

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        public int CategoryId { get; set; }

        public string? AssignedToUserId { get; set; }

        [ValidateNever]
        public List<SelectListItem> Categories { get; set; } = new();

        [ValidateNever]
        public List<SelectListItem> Users { get; set; } = new();
    }

    public class CategorySummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UserSummaryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
    }

    public class TaskAssignmentViewModel
    {
        public string? AssignedToUserId { get; set; }
    }
}
