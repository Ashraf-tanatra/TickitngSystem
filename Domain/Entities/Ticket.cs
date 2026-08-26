using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        public int TicketId { get; } //Auto Generated 
        public int ProjectId { get; set; } // Foreign Key
        public int EmployeeId { get; set; } // Foreign Key
        public DateOnly? DueTo { get; set; }
        // Navigation property for the project associated with the ticket
        public Project Project { get; set; } = null!;
        // Navigation property for the employee assigned to the ticket
        public Employee? Employee { get; set; }
        public string? Description { get; set; }
        public TicketPriority Priority { get; set; }
        public string TicketTitle { get; set; } = null!;
        public int TicketCreatedById { get; set; } // Foreign Key
        // Navigation property for the employee who created the ticket
        public Employee TicketCreatedBy { get; set; } = null!;
        // Navigation property for the ticket history associated with the ticket
        public ICollection<TicketHistory>? TicketHistories { get; set; }
        // Navigation property for the ticket attachments associated with the ticket
        public ICollection<TicketAttachments>? AttachmentURL { get; set; }
        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public override string ToString() => $"{TicketId} Ticket Title: {TicketTitle} Created On: {CreatedAt}\n" +
                   $"Ticket Status: {TicketStatus}";
    }
}