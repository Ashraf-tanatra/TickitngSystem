namespace ApplicationServices.DTOs.Ticket
{
    public class TicketResponse
    {
        public Guid TicketId { get; set; }



        public string TicketTitle { get; set; } = string.Empty;

        public DateOnly? DueTo { get; set; }

        public string TicketStatus { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid ProjectId { get; set; }

        public string? ProjectName { get; set; } 
        public string? EmployeeName { get; set; }

        public IEnumerable<TicketAttachmentResponse> Attachments { get; set; } =
            Enumerable.Empty<TicketAttachmentResponse>();
    }
}
