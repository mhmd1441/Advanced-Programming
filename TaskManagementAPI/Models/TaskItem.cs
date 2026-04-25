namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }
        public string Priority { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string? AssignedToUserId { get; set; }
        public ApplicationUser? AssignedToUser { get; set; }
    }
}