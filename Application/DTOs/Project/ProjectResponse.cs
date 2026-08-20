namespace ApplicationServices.DTOs.Project
{
    public class ProjectResponse
    {
        public int Id { get; set; }

        public string ProjectName { get; set; } = null!;

        public string? ProjectDescription { get; set; }

        public int ProjectManagerId { get; set; } // ?

        public string? ProjectManagerName { get; set; }

        public int EmployeeCount { get; set; } // we need also the Employees that work in a project

        public int TicketCount { get; set; } // we need also the Tickets that's in a project
    }
}