using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        // Used by Employee
        public required int TicketId { get; set; } //Auto Generated 
        public required string TicketTitle { get; set; }
        public DateTime? DueTo { get; set; }

        private TicketStatus ticketStatus = TicketStatus.Pending;
        public Employee? TicketAssignedToEmployee { get; set; }
        public string? Description { get; set; }

        private TicketPriority Priority = TicketPriority.Low;
        public string? Type { get; set; }

        // Used by Project Manager
        public required int ProjectId { get; set; }

        private DateTime CreatedTime = DateTime.Now;

        // Used by Employee and project manager
        public void SetAsOnProgress() => ticketStatus = TicketStatus.OnProgress;
        public void TicketCompleted() => ticketStatus = TicketStatus.Completed;
        public void TicketCancelled() => ticketStatus = TicketStatus.Cancelled;
        public void TicketDone() => ticketStatus = TicketStatus.Done;

        // Used by Project Manager only
        public void SetPriorityToHigh() => Priority = TicketPriority.High;
        public void SetPriorityToLow() => Priority = TicketPriority.Low;
        public void SetPriorityToMedium() => Priority = TicketPriority.Medium;


        public override string ToString()
        {
            return $"{TicketId} Ticket Title: {TicketTitle} Created On:{CreatedTime}\n" +
                $"Ticket Status: {ticketStatus}";
        }

    }
}
