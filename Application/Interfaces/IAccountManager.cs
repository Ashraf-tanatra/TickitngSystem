using ApplicationServices.DTOs.Account;
using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IAccountManager
    {
        Task<AccountResponse> CreateAccountAsync(
            CreateAccountRequest request);

        Task<AccountResponse?> GetByEmailAsync(
            string email);

        Task<Account?> GetEntityByEmailAsync(
            string email);

        Task<bool> ExistsAsync(
            string email);

        Task<bool> DeleteAsync(
            string email);

        Task<bool> SoftDeleteAsync(
            string email);

        Task<bool> ReactivateAsync(
            string email);

        Task<AccountResponse?> UpdateAsync(
            int id,
            UpdateAccountRequest request);

        bool ValidEmailFormat(string email);

        bool PasswordFormat(string password);
    }
}