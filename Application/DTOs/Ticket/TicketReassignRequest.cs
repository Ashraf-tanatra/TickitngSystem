namespace ApplicationServices.DTOs.Ticket
{
    public class TicketReassignRequest
    {
        public int ActionByEmployeeId { get; set; }

        public int ToEmployeeId { get; set; }

        public string? Note { get; set; }
    }
}
