namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public required string ProjectName { get; set; }

        public string? ProjectDescription { get; set; }

        // Employees + their roles in this project
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
            = new List<ProjectEmployee>();

        // Tickets belonging to this project
        public ICollection<Ticket> ProjectTickets { get; set; }
            = new List<Ticket>();
    }
}