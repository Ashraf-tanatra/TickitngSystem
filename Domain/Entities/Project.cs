using Domain.Enum;

namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public DateOnly? EndAt { get; set; }
        public DateOnly? StartedAt { get; set; }
        public string? ProjectDescription { get; set; }
        public string ProjectName { get; set; } = null!;
        public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Active;


        // Foreign Key
        public int ProjectManagerId { get; set; }
        public Employee? ProjectManager { get; private set; }
        public ICollection<Ticket>? ProjectTickets { get; set; }
        public ICollection<ProjectEmployee>? ProjectEmployees { get; set; }

        public override string ToString()
        {
            return $"Project Id: {Id}\nProject Name: {ProjectName}\n" +
                   $"Details: {ProjectDescription}";
        }
    }
}