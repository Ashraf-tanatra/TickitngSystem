namespace ApplicationServices.DTOs.Account
{
    public class VerifyEmailRequest
    {
        public string Code { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}