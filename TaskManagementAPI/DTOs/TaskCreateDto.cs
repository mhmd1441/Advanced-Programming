namespace TaskManagementAPI.DTOs
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public int CategoryId { get; set; }
        public string? AssignedToUserId { get; set; }
    }
}