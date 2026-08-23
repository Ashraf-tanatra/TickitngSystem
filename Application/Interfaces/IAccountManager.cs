using ApplicationServices.DTOs;
using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IAccountManager
    {
        AccountResponse CreateAccount(CreateAccountRequest request);

        AccountResponse? GetByEmail(string email);

        Account? GetEntityByEmail(string email);

        bool Exists(string email);

        bool Delete(string email);

        bool ValidEmailFormat(string email);

        bool PasswordFormat(string password);

        AccountResponse? Update( int id, UpdateAccountRequest request);
    }
}