using Domain.Entities;

public class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Account()
    {

    }
}