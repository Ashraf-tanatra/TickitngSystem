using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        public int? TicketId { get; set; }
        public string? TicketTitle { get; set; }
        private DateTime CreatedTime = DateTime.Now;

        private TicketSatus ticketStatus = TicketSatus.Pending;
        public IEnumerable<Employee> EmployeeAssignedTicket { get; set; }
        public int ProjectId { get; set; }

        public void SetAsOnProgress() => ticketStatus = TicketSatus.OnProgress;
        public void TicketCompleted() => ticketStatus = TicketSatus.Completed;
        public void TicketCancelled() => ticketStatus = TicketSatus.Cancelled;

        public override string ToString()
        {
            return $"{TicketId} Ticket Title: {TicketTitle} Created On:{CreatedTime}\n" +
                $"Ticket Status: {ticketStatus}";
        }
    }
}
