namespace Domain.Entities
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; } // Foreign Key
        public Project Project { get; set; } = null!;
        public int EmployeeId { get; set; }  // Foreign Key
        public Employee Employee { get; set; } = null!;

        public string? Role { get; set; }
    }
}