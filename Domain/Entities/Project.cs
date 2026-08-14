namespace Domain.Entities
{
    public class Project
    {
        public required int Id { get; set; } //Auto Generated
        public required string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }

        public int ProjectManagerId { get; private set; }

        public Employee? ProjectManager { get; private set; } = null;

        public ICollection<Ticket>? ProjectTickets { get; set; }

        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();

        public override string ToString() => $"Project Id: {Id}\nProject Name: {ProjectName}\nDetails:{ProjectDescription}";
    }
}