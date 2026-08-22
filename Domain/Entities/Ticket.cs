using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        // Used by Employee
        public int TicketId { get; } //Auto Generated 
        public required string TicketTitle { get; set; }

        public DateOnly? DueTo { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;
        public TicketPriority Priority { get; set; }
        public string? Description { get; set; }

        public Employee? Employee { get; set; }
        public int EmployeeId { get; set; }

        public int TicketCreatedById { get; set; }
        public Employee TicketCreatedBy { get; set; } = null!;

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public ICollection<TicketHistory>? TicketHistories { get; set; }
        public ICollection<TicketAttachments>? AttachmentURL { get; set; }

        public override string ToString() => $"{TicketId} Ticket Title: {TicketTitle} Created On: {CreatedAt}\n" +
                   $"Ticket Status: {TicketStatus}";
    }
}