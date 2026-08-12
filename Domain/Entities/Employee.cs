using Domain.Enum;
namespace Domain.Entities;


public class Employee
{
    public int Id { get; private set; }

    public string FName { get; private set; }
    public string LName { get; private set; }

    public string Phone { get; private set; }
    public string Gender { get; private set; }
    public Account Account { get; private set; }
    public IEnumerable<Ticket>? Tickets { get; set; }

    private EmployeeRole Role { get; }

    public bool IsAvailable { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;

    private Employee() { }

    public Employee(
        string fName,
        string lName,
        string phone,
        string gender,
        Account account,
        EmployeeRole role)
    {
        FName = fName;
        LName = lName;
        Phone = phone;
        Gender = gender;
        Role = role;
        IsAvailable = true;
    }
}