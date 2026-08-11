namespace Domain.Entities
{
    public class Account
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public Account(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
