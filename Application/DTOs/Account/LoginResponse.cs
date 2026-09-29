namespace ApplicationServices.DTOs.Account
{
    public class LoginResponse
    {
        public string Email { get; set; } = string.Empty;

        public string FName { get; set; } = string.Empty;

        public string LName { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public string AccessToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresAtUtc { get; set; }
    }
}
