using Domain.Enum;

namespace ApplicationServices.DTOs.Employee;

public class UpdateEmployeeRequest
{
    public string FName { get; set; } = null!;

    public string LName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public Gender Gender { get; set; }

    //public int RoleId { get; set; }

    public bool IsAvailable { get; set; } // ?
}
