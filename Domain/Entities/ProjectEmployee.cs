namespace Domain.Entities
{
    public class ProjectEmployee
    {
        // Foreign Key
        public int ProjectId { get; set; }
        // Foreign Key
        public int EmployeeId { get; set; }
        public Project Project { get; set; }
        public Employee Employee { get; set; }

        public string? Role { get; set; }
    }
}