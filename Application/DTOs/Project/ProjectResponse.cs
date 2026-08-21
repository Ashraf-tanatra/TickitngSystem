namespace ApplicationServices.DTOs.Project
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? ProjectDescription { get; set; }
        public string ProjectStatus { get; set; } = null!;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? EmployeeRole { get; set; }

        public int ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }

        //public int EmployeeCount { get; set; } 
        //public int TicketCount { get; set; } 
    }
}