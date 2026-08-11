namespace Domain.Entities;

public class Employee
{
    public int Id { get; private set; }

    public string FName { get; private set; }
    public string LName { get; private set; }

    public string Phone { get; private set; }
    public string Gender { get; private set; }

    public Role Role { get; private set; }

    public bool IsAvailable { get; private set; }
    public bool IsDeleted { get; private set; }

    private Employee() { }

    public Employee(
        string fName,
        string lName,
        string phone,
        string gender,
        Role role)
    {
        FName = fName;
        LName = lName;
        Phone = phone;
        Gender = gender;
        Role = role;
    }
}