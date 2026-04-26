using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTOs
{
    public class CategoryCreateDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}