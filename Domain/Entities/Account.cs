using Domain.Entities;

public class Account
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    //public bool EmailConfirmed { get; set; } = false;

    //public string? VerificationCode { get; set; }

    //public DateTime? VerificationCodeExpiresAt { get; set; }
}