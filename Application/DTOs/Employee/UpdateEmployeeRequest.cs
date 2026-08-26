using Domain.Enum;

namespace ApplicationServices.DTOs.Employee;

public class UpdateEmployeeRequest
{
    public Gender Gender { get; set; }
    public string FName { get; set; } = null!;
    public string LName { get; set; } = null!;
    public string Phone { get; set; } = null!;
}
