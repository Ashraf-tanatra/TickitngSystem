namespace ApplicationServices.DTOs.Account
{
    public class VerifyResetCodeRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }
}
