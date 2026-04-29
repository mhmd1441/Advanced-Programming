using System.ComponentModel.DataAnnotations;

namespace TaskManagementMvc.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
