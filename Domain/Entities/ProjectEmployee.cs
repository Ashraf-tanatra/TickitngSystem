namespace Domain.Entities
{
    public class ProjectEmployee
    {
        private ProjectEmployee()
        {
        }

        public string? Role { get; private set; }
        public Guid ProjectId { get; private set; }
        public Guid EmployeeId { get; private set; }
        public Project Project { get; private set; } = null!;
        public Employee Employee { get; private set; } = null!;

        public static ProjectEmployee Create(
            Guid projectId,
            Guid employeeId,
            string? role)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);

            if (employeeId == Guid.Empty)
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            return new ProjectEmployee
            {
                ProjectId = projectId,
                EmployeeId = employeeId,
                Role = string.IsNullOrWhiteSpace(role) ? null : role.Trim()
            };
        }

        public void ChangeRole(string? role)
        {
            Role = string.IsNullOrWhiteSpace(role) ? null : role.Trim();
        }
    }
}
