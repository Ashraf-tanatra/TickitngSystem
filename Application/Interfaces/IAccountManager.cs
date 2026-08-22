using ApplicationServices.DTOs.Account;

namespace ApplicationServices.Interfaces
{
    public interface IAccountManager
    {
        AccountResponse CreateAccount(CreateAccountRequest request);

        AccountResponse? GetByEmail(string email);

        bool Exists(string email);

        bool Delete(string email);
    }
}