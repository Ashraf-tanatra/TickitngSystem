namespace ApplicationServices.DTOs.Account
{
    public class ResetPasswordRequest
    {
        public string Code { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}