using Domain.Enum;

namespace ApplicationServices.DTOs
{
    public class SignUpRequest
    {
        public string? FName { get; set; }

        public string? LName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public Gender Gender { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        public bool AcceptTerms { get; set; }
    }
}