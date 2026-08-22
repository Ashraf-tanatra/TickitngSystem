using Domain.Enum;

public class CreateTicketRequest
{
    public string TicketTitle { get; set; } = null!;
    public DateOnly? DueTo { get; set; }

    //public TicketStatus TicketStatus { get; } = TicketStatus.Pending;
    public TicketPriority Priority { get; set; }


    public string? Description { get; set; }
    //public string? AttachmentURL { get; set; }

    public int EmployeeId { get; set; }
    public int TicketCreatedById { get; set; }
    public int ProjectId { get; set; }
}