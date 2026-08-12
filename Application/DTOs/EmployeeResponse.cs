namespace ApplicationServices.DTOs;

public class EmployeeResponse
{
    public int Id { get; set; }

    public string FName { get; set; } = string.Empty;

    public string LName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public string RoleName { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
}