namespace ApplicationServices.DTOs.Ticket
{
    public class TicketResponse
    {
        public int TicketId { get; set; }
        public int ProjectId { get; set; } // Foreign key to the Project
        public int EmployeeId { get; set; } // Foreign key to the Employee
        public DateOnly? DueTo { get; set; }
        public string? Description { get; set; }
        public string? ProjectName { get; set; }
        public string? EmployeeName { get; set; }
        public string Priority { get; set; } = null!;
        public string TicketTitle { get; set; } = null!;
        public string TicketStatus { get; set; } = null!;
    }
}