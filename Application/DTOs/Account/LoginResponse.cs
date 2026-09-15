namespace ApplicationServices.DTOs.Account
{
    public class LoginResponse
    {
        public Guid EmployeeId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FName { get; set; } = string.Empty;

        public string LName { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }
    }
}
