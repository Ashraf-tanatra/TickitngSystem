namespace ApplicationServices.DTOs.Ticket
{
    public class TicketReassignRequest
    {
        public Guid ToEmployeeId { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(ErrorShared.Ticket.DescriptionMaxLength)]
        public string? Note { get; set; }
    }
}
