using Domain.Enum;

namespace ApplicationServices.DTOs.Account
{
    public class SignUpRequest
    {
        public string FName { get; set; } = string.Empty;

        public string LName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public bool AcceptTerms { get; set; }
    }
}
