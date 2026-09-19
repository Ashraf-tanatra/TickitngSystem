using Domain.Enum;

namespace ApplicationServices.DTOs.Account
{
    public class SignUpRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string FName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string LName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(16, MinimumLength = 9)]
        public string Phone { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(128, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(128)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool AcceptTerms { get; set; }
    }
}
