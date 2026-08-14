using Domain.Enum;

namespace Domain.Entities;

// Deleted Employees from Project because we use ProjectEmployee now.
// Updated ProjectEmployee to connect Project and Employee.
// Added ProjectEmployees navigation property in Project and Employee.
public class Employee
{
    public int Id { get; private set; }

    public string FName { get; private set; }
    public string LName { get; private set; }
    public string Phone { get; private set; }
    public string Gender { get; private set; }
    public Account Account { get; private set; }
    public EmployeeRole Role { get; }

    public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();

    // public bool IsAvailable { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public ICollection<Ticket>? Tickets { get; set; }
    public ICollection<Project>? Projects { get; set; }
    public int ProjId { get; private set; }


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
        // IsAvailable = true;
    }
}