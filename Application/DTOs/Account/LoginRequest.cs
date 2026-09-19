namespace ApplicationServices.DTOs.Account
{
    public class LoginRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string Email { get; set; } = null!;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(128)]
        public string Password { get; set; } = null!;
    }
}
