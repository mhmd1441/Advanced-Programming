namespace TaskManagementAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<TaskItem> Tasks { get; set; }
    }
}