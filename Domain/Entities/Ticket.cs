using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        // Used by Employee
        public required int TicketId { get; set; } //Auto Generated 
        public required string TicketTitle { get; set; }

        public DateTime? DueTo { get; set; }
        public DateTime CreatedTime { get; } = DateTime.Now;

        public TicketStatus ticketStatus { get; private set; } = TicketStatus.Pending;
        public TicketPriority Priority { get; private set; } = TicketPriority.Low;

        public string? Description { get; set; }

        public Employee? Employee { get; set; }
        public int EmployeeId { get; set; }
        public required int ProjectId { get; set; }
        //public Employee TicketCreatedBy { get; set; }
        public Project Project { get; set; }


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
