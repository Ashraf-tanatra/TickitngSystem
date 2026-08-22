namespace ApplicationServices.DTOs.Ticket
{
    public class TicketResponse
    {
        public int TicketId { get; set; }
        public string TicketTitle { get; set; } = null!;
        public DateOnly? DueTo { get; set; }
        public string? Description { get; set; }

        public string TicketStatus { get; set; } = null!;
        public string Priority { get; set; } = null!;

        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }

        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
    }
}