namespace ApplicationServices.DTOs.Employee;

public sealed class EmployeeSummaryResponse
{
    public Guid Id { get; set; }

    public string FName { get; set; } = string.Empty;

    public string LName { get; set; } = string.Empty;

    public string? ProfileImageUrl { get; set; }
}
