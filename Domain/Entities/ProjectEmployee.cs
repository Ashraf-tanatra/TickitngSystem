namespace Domain.Entities
{
    public class ProjectEmployee
    {
        private ProjectEmployee()
        {
        }

        public string? Role { get; private set; }
        public int ProjectId { get; private set; }
        public int EmployeeId { get; private set; }
        public Project Project { get; private set; } = null!;
        public Employee Employee { get; private set; } = null!;

        public static ProjectEmployee Create(
            int projectId,
            int employeeId,
            string? role)
        {
            if (projectId <= 0)
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);

            if (employeeId <= 0)
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
