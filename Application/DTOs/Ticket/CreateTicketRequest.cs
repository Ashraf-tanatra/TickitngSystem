using Domain.Enum;

public class CreateTicketRequest
{
    public int ProjectId { get; set; }
    public int EmployeeId { get; set; }
    public DateOnly? DueTo { get; set; }
    public string? Description { get; set; }
    public int TicketCreatedById { get; set; }
    public TicketPriority Priority { get; set; }
    public string TicketTitle { get; set; } = null!;
}