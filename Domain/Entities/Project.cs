using Domain.Enum;

namespace Domain.Entities
{
    public class Project : BaseEntity
    {
        private readonly List<Ticket> _projectTickets = new();
        private readonly List<ProjectEmployee> _projectEmployees = new();

        private Project()
        {
        }

        public DateOnly? EndAt { get; private set; }
        public DateOnly? StartedAt { get; private set; }
        public string? ProjectDescription { get; private set; }
        public string ProjectName { get; private set; } = string.Empty;
        public ProjectStatus ProjectStatus { get; private set; } = ProjectStatus.Active;

        //Relationship for EF_core
        public int ProjectManagerId { get; private set; }
        public Employee ProjectManager { get; private set; } = null!;
        public IReadOnlyCollection<Ticket> ProjectTickets => _projectTickets;
        public IReadOnlyCollection<ProjectEmployee> ProjectEmployees => _projectEmployees;

        public static Project Create(
            string projectName,
            string? projectDescription,
            int projectManagerId,
            DateOnly? startedAt,
            DateOnly? endAt)
        {
            if (projectManagerId <= 0)
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            var project = new Project
            {
                ProjectManagerId = projectManagerId,
                ProjectStatus = ProjectStatus.Active
            };

            project.UpdateDetails(projectName, projectDescription, startedAt, endAt);
            return project;
        }

        public void UpdateDetails(
            string projectName,
            string? projectDescription,
            DateOnly? startedAt,
            DateOnly? endAt)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException(ErrorShared.Project.ProjectNameRequired);

            ProjectName = projectName.Trim();
            ProjectDescription = string.IsNullOrWhiteSpace(projectDescription)
                ? null
                : projectDescription.Trim();
            StartedAt = startedAt;
            EndAt = endAt;
            Touch();
        }

        public void ChangeStatus(ProjectStatus status)
        {
            if (!System.Enum.IsDefined(status))
                throw new ArgumentException(ErrorShared.Project.InvalidStatus);

            ProjectStatus = status;
            Touch();
        }

        public override string ToString() =>
            $"Project Id: {Id}\nProject Name: {ProjectName}\nDetails: {ProjectDescription}";
    }
}
