public class EmployeeResponse
{
    public int Id { get; set; }

    public string FName { get; set; } = string.Empty;

    public string LName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
}