using Domain.Enum;

namespace ApplicationServices.DTOs.Ticket;

public class CreateTicketRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(ErrorShared.Ticket.TicketTitleMaxLength)]
    public string TicketTitle { get; set; } = null!;

    public DateOnly? DueTo { get; set; }

    [System.ComponentModel.DataAnnotations.EnumDataType(typeof(TicketPriority))]
    public TicketPriority Priority { get; set; }

    [System.ComponentModel.DataAnnotations.StringLength(ErrorShared.Ticket.DescriptionMaxLength)]
    public string? Description { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ProjectId { get; set; }
}
