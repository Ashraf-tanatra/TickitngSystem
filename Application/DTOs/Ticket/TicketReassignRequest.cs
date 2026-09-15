namespace ApplicationServices.DTOs.Ticket
{
    public class TicketReassignRequest
    {
        public Guid ToEmployeeId { get; set; }

        public string? Note { get; set; }
    }
}
