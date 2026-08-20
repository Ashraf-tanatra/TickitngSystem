using Domain.Enum;

namespace Domain.Entities
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        // Employee's role in THIS project
        public ProjectRole Role { get; set; }
    }
}