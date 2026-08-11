namespace Domain.Entities
{
    public class Project
    {
        public required int Id { get; set; } //Auto Generated
        public required string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public int ProjectManagerId { get; private set; }
        public IEnumerable<Employee>? Employees { get; set; }
        public IEnumerable<Ticket>? ProjectTickets { get; set; }

        public override string ToString() => $"Project Id: {Id}\nProject Name: {ProjectName}\nDetails:{ProjectDescription}";

    }
}
