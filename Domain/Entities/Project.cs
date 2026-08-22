using Domain.Enum;

namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public required string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Active;
        public DateOnly? StartedAt { get; set; }
        public DateOnly? EndAt { get; set; }


        // Foreign Key
        public int ProjectManagerId { get; set; }
        public Employee? ProjectManager { get; private set; }
        public ICollection<Ticket>? ProjectTickets { get; set; }
        public ICollection<ProjectEmployee>? ProjectEmployees { get; set; }

        public override string ToString()
        {
            return $"Project Id: {Id}\n" +
                   $"Project Name: {ProjectName}\n" +
                   $"Details: {ProjectDescription}";
        }
    }
}