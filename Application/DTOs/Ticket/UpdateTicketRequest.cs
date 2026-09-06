public class UpdateTicketRequest
{
    public string TicketTitle { get; set; } = string.Empty;

    public DateOnly? DueTo { get; set; }

    public string? Description { get; set; }

    public int EmployeeId { get; set; }
}