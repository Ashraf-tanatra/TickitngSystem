using Domain.Enum;

namespace Domain.Entities
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }

        public int EmployeeId { get; set; }

        public Project Project { get; set; }

        public Employee Employee { get; set; }

        public EmployeeRole Role { get; set; }
    }
}