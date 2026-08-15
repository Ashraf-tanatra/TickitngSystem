namespace ApplicationServices.DTOs
{
    public class TicketResponse
    {
        public int TicketId { get; set; }

        public string TicketTitle { get; set; } = string.Empty;

        public DateTime? DueTo { get; set; }

        public string TicketStatus { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int EmployeeId { get; set; }

        public int ProjectId { get; set; }
    }
}