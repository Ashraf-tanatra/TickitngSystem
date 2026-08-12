namespace ApplicationServices.Interfaces
{

    public interface IAccountManager
    {
        Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request);

        Task<AccountResponse?> GetByEmailAsync(string email);

        Task<bool> ExistsAsync(string email);

        Task<bool> DeleteAsync(string email);
    }
}