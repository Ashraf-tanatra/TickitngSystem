using Domain.Enum;

public class EmployeeResponse
{
    public int Id { get; set; }

    public string FName { get; set; } 

    public string LName { get; set; } 

    public string Phone { get; set; } 

    public Gender Gender { get; set; }

    public bool IsDeleted { get; set; }
}