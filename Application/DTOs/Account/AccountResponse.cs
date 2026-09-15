namespace ApplicationServices.DTOs.Account
{
    public class AccountResponse //?
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public Guid EmployeeId { get; set; }
    }
}