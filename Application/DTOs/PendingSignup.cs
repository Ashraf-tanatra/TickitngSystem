using Domain.Enum;

public class PendingSignup
{
    public int Id { get; set; }

    public string FName { get; set; } = null!;
    public string LName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public Gender Gender { get; set; }
    public string PasswordHash { get; set; } = null!;

    public string VerificationCode { get; set; } = null!;
    public DateTime VerificationCodeExpiresAt { get; set; }
}