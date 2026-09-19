namespace ApplicationServices.DTOs.Ticket
{
    public class TicketActionRequest
    {
        [System.ComponentModel.DataAnnotations.StringLength(ErrorShared.Ticket.DescriptionMaxLength)]
        public string? Note { get; set; }
    }
}
