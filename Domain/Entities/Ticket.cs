using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        // Used by Employee
        public int TicketId { get; } //Auto Generated 
        public required string TicketTitle { get; set; }

        public DateTime? DueTo { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;
        public TicketPriority Priority { get; set; } = TicketPriority.Low;

        public string? Description { get; set; }
        public string? AttachmentURL { get; set; }

        public Employee? Employee { get; set; }
        public int EmployeeId { get; set; }
        public int TicketCreatedById { get; set; }
        public Employee TicketCreatedBy { get; set; }

        public required int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();

        // Used by Employee and project manager
        public void SetAsOnProgress() => TicketStatus = TicketStatus.InProgress;
        public void TicketCompleted() => TicketStatus = TicketStatus.Completed;
        public void TicketCancelled() => TicketStatus = TicketStatus.Cancelled;
        public void TicketDone() => TicketStatus = TicketStatus.Done;
        public void TicketReOpened() => TicketStatus = TicketStatus.Reopened;

        // Used by Project Manager only
        public void SetPriorityToHigh() => Priority = TicketPriority.High;
        public void SetPriorityToLow() => Priority = TicketPriority.Low;
        public void SetPriorityToMedium() => Priority = TicketPriority.Medium;

        public override string ToString()
        {
            return $"{TicketId} Ticket Title: {TicketTitle} Created On: {CreatedTime}\n" +
                   $"Ticket Status: {TicketStatus}";
        }
    }
}