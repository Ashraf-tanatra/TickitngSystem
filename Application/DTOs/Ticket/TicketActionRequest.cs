namespace ApplicationServices.DTOs.Ticket
{
    public class TicketActionRequest
    {
        public Guid ActionByEmployeeId { get; set; }

        public string? Note { get; set; }
    }
}
