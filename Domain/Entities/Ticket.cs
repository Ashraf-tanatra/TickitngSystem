using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        public int TicketId { get; } // Primary Key
        public DateOnly? DueTo { get; set; }
        public string? Description { get; set; }
        public TicketPriority Priority { get; set; }
        public required string TicketTitle { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;


        // RelationShips for EF_Core 
        // Project
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        // Current assigned Employee
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; } = null!;

        // Employee who created the ticket
        public int TicketCreatedById { get; set; }
        public Employee TicketCreatedBy { get; set; } = null!;

        // Ticket Attachments
        public ICollection<TicketAttachments> AttachmentURL { get; set; } = new List<TicketAttachments>();

        // Ticket History
        public ICollection<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();

        public override string ToString() => $"{TicketId} Ticket Title: {TicketTitle} " +
            $"Created On: {CreatedAt}\nTicket Status: {TicketStatus}";

    }
}