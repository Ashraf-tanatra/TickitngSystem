using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        public int TicketId { get; set; }

        public required string TicketTitle { get; set; }

        public DateTime? DueTo { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;

        public TicketPriority Priority { get; set; } = TicketPriority.Low;

        public string? Description { get; set; }
        // Current assigned Employee
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        // Employee who created the ticket
        public int TicketCreatedById { get; set; }
        public Employee TicketCreatedBy { get; set; } = null!;
        // Project
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        // Ticket lifecycle
        public ICollection<TicketHistory> TicketHistories { get; set; }= new List<TicketHistory>();
        // Status operations
        public void SetAsOnProgress() => TicketStatus = TicketStatus.InProgress;
        public void TicketCompleted()=> TicketStatus = TicketStatus.Completed;
        public void TicketCancelled()=> TicketStatus = TicketStatus.Cancelled;public void TicketDone()=> TicketStatus = TicketStatus.Done;
        // Priority operations
        public void SetPriorityToHigh()=> Priority = TicketPriority.High;
        public void SetPriorityToLow()=> Priority = TicketPriority.Low;
        public void SetPriorityToMedium()
            => Priority = TicketPriority.Medium;


        public override string ToString()
        {
            return $"{TicketId} Ticket Title: {TicketTitle} Created On: {CreatedTime}\n" +
                   $"Ticket Status: {TicketStatus}";
        }
    }
}