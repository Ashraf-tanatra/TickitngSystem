namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public required string ProjectName { get; set; }

        public string? ProjectDescription { get; set; }

        public int ProjectManagerId { get; set; }

        public Employee? ProjectManager { get; private set; }

        public ICollection<Ticket> ProjectTickets { get; set; }
            = new List<Ticket>();

        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
            = new List<ProjectEmployee>();

        public override string ToString()
        {
            return $"Project Id: {Id}\n" +
                   $"Project Name: {ProjectName}\n" +
                   $"Details: {ProjectDescription}";
        }
    }
}