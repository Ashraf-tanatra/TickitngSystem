namespace ApplicationServices.DTOs
{
    public class EmployeeProjectResponse
    {
        public int Id { get; set; }

        public string? ProjectName { get; set; }

        public string? ProjectDescription { get; set; }

        public string? Role { get; set; }

        public int EmployeeCount { get; set; }

        public int TicketCount { get; set; }
    }
}