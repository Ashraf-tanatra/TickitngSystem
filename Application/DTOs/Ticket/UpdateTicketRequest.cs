public class UpdateTicketRequest
{
    public string TicketTitle { get; set; } = string.Empty;

    public DateOnly? DueTo { get; set; }

    public string? Description { get; set; }

    public Guid EmployeeId { get; set; }
}