using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        // Primary Key
        public int TicketId { get; }

        public required string TicketTitle { get; set; }

        public DateOnly? DueTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public TicketStatus TicketStatus { get; set; }= TicketStatus.Pending;

        public TicketPriority Priority { get; set; }

        public string? Description { get; set; }
        // Ticket Attachments
        public ICollection<TicketAttachments> TicketAttachments { get; set; } = new List<TicketAttachments>();
        // Current assigned Employee
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }= null!;
        // Employee who created the ticket
        public int TicketCreatedById { get; set; }
        public Employee TicketCreatedBy { get; set; }= null!;
        // Project
        public int ProjectId { get; set; }
        public Project Project { get; set; }= null!;
        // Ticket History
        public ICollection<TicketHistory> TicketHistories { get; set; }= new List<TicketHistory>();
        public override string ToString()
        {return $"{TicketId} Ticket Title: {TicketTitle} " +$"Created On: {CreatedAt}\n" +$"Ticket Status: {TicketStatus}";
        }
    }
}