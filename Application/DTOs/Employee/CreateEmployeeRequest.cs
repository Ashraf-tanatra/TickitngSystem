using Domain.Enum;

namespace ApplicationServices.DTOs.Employee;

public class CreateEmployeeRequest
{
    public Gender Gender { get; set; }
    public string FName { get; set; } = null!;
    public string LName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
