namespace ApplicationServices.DTOs.Account
{
    public class UpdateAccountRequest
    {
        [System.ComponentModel.DataAnnotations.EmailAddress]
        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string? Email { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(128)]
        public string? CurrentPassword { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(128, MinimumLength = 8)]
        public string? NewPassword { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(128)]
        public string? ConfirmNewPassword { get; set; }
    }
}
