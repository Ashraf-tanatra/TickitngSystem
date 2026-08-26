public class UpdateTicketRequest
{
    public DateOnly? DueTo { get; set; }
    public int EmployeeId { get; set; } // Foreign key to the Employee
    public string? Description { get; set; }
    public string TicketTitle { get; set; } = null!;
}