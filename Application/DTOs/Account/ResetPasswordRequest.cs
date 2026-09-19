namespace ApplicationServices.DTOs.Account
{
    public class ResetPasswordRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(128, MinimumLength = 8)]
        public string NewPassword { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(128)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
