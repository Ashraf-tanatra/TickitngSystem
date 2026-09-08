namespace ApplicationServices.DTOs.Ticket
{
    public class TicketActionRequest
    {
        public int ActionByEmployeeId { get; set; }

        public string? Note { get; set; }
    }
}
