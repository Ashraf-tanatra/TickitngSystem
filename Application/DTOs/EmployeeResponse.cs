using Domain.Enum;

public class EmployeeResponse
{
    public int Id { get; set; }

    public string FName { get; set; } = null;

    public string LName { get; set; } = null;

    public string Phone { get; set; } = null;

    public Gender Gender { get; set; }

    public bool IsDeleted { get; set; }
}