using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        public int? TicketId { get; set; }
        public string? TicketTitle { get; set; }
        public DateTime CreatedTime { get; set; }

        private TicketSatus ticketStatus = TicketSatus.Pending;
        public IEnumerable<Employee> EmployeeAssignedTicket { get; set; }
        public int ProjectId { get; set; }


    }
}
