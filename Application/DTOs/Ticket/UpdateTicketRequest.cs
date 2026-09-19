namespace ApplicationServices.DTOs.Ticket;

public class UpdateTicketRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(ErrorShared.Ticket.TicketTitleMaxLength)]
    public string TicketTitle { get; set; } = string.Empty;

    public DateOnly? DueTo { get; set; }

    [System.ComponentModel.DataAnnotations.StringLength(ErrorShared.Ticket.DescriptionMaxLength)]
    public string? Description { get; set; }

    public Guid EmployeeId { get; set; }
}
