using Domain.Enum;

namespace ApplicationServices.DTOs.Ticket;

public class CreateTicketRequest
{
    public string TicketTitle { get; set; } = null!;

    public DateOnly? DueTo { get; set; }

    public TicketPriority Priority { get; set; }

    public string? Description { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid TicketCreatedById { get; set; }

    public Guid ProjectId { get; set; }
}
