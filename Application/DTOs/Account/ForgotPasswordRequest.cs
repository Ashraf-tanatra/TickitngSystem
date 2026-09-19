namespace ApplicationServices.DTOs.Account
{
    public class ForgotPasswordRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string Email { get; set; } = string.Empty;
    }
}
