namespace ApplicationServices.DTOs.Project
{
    public class ProjectResponse
    {
        public int Id { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string? ProjectDescription { get; set; }

        public int ProjectManagerId { get; set; }

        public string? ProjectManagerName { get; set; }

        public int EmployeeCount { get; set; }

        public int TicketCount { get; set; }
    }
}