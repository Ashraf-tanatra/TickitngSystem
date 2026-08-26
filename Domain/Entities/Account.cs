using Domain.Entities;

public class Account
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsDeleted { get; set; } = false;

    //From Verify Account
    public bool IsEmailVerified { get; set; } = false;
    public string? VerificationCode { get; set; }
    public DateTime? VerificationCodeExpiresAt { get; set; }

    //For Forget Password
    public string? PasswordResetCode { get; set; }
    public DateTime? PasswordResetCodeExpiresAt { get; set; }
}