namespace ApplicationServices.DTOs;

public class UpdateEmployeeRequest
{
    public string FName { get; set; } = string.Empty;

    public string LName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public bool IsAvailable { get; set; }
}
