using Domain.Enum;

namespace ApplicationServices.DTOs.Employee;

public class UpdateEmployeeRequest
{
    public string FName { get; set; } = string.Empty;

    public string LName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public int RoleId { get; set; }

    public bool IsAvailable { get; set; }
}
