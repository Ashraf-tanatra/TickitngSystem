using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;

namespace ApplicationServices.Services
{
    public class AccountManager : IAccountManager
    {
        public AccountResponse CreateAccount(CreateAccountRequest request)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string email)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string email)
        {
            throw new NotImplementedException();
        }

        public AccountResponse? GetByEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}