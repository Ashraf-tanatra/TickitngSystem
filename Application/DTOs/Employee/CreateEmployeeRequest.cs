using Domain.Enum;

namespace ApplicationServices.DTOs.Employee;

public class CreateEmployeeRequest
{
    public string FName { get; set; } = null!;

    public string LName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public Gender Gender { get; set; }

    //public int RoleId { get; set; } // deleted

    // these are in account dtos ??
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
