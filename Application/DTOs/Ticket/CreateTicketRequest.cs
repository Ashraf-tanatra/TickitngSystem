using Domain.Enum;

public class CreateTicketRequest
{
    public string TicketTitle { get; set; } = null!;

    public DateOnly? DueTo { get; set; }

    public TicketPriority Priority { get; set; }

    public string? Description { get; set; }
    public string? AttachmentURL { get; set; }

    public Guid EmployeeId { get; set; }
    public Guid TicketCreatedById { get; set; }

    public Guid ProjectId { get; set; }
}
