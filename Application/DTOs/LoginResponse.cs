namespace ApplicationServices.DTOs
{
    public class LoginResponse
    {
        public int EmployeeId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}