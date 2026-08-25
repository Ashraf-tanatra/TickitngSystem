namespace Domain.Entities
{
    public class ProjectEmployee
    {
        public string? Role { get; set; }
        public int ProjectId { get; set; } // Foreign Key
        public int EmployeeId { get; set; }  // Foreign Key
        public Project Project { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}