using ApplicationServices.Interfaces;
namespace Domain.EntityManager
{
    public class AccountManager : IAccountManager
    {
        Task<AccountResponse> IAccountManager.CreateAccountAsync(CreateAccountRequest request)
        {
            throw new NotImplementedException();
        }

        Task<bool> IAccountManager.DeleteAsync(string email)
        {
            throw new NotImplementedException();
        }

        Task<bool> IAccountManager.ExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        Task<AccountResponse?> IAccountManager.GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
