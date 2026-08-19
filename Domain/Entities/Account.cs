using Domain.Entities;

public class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; }

}