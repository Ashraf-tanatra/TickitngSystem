namespace ApplicationServices.DTOs.Account
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Email { get; set; } = null!;
    }
}