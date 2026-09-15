namespace ApplicationServices.DTOs.Ticket
{
    public class TicketReassignRequest
    {
        public Guid ActionByEmployeeId { get; set; }

        public Guid ToEmployeeId { get; set; }

        public string? Note { get; set; }
    }
}
