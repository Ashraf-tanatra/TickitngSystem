using Domain.Enum;

public class CreateTicketRequest
{
    public string TicketTitle { get; set; } = string.Empty;

    public DateTime? DueTo { get; set; }

    public TicketPriority Priority { get; set; }

    public string? Description { get; set; }

    public int EmployeeId { get; set; }

    public int ProjectId { get; set; }
}