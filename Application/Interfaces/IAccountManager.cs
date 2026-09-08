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

        Task<bool> SoftDeleteAsync(
            DeactivateAccountRequest request);

        Task<bool> ReactivateAsync(
            string email);

        Task<AccountResponse?> UpdateAsync(
            int id,
            UpdateAccountRequest request);

        // EMAIL VERIFICATION
        Task SetVerificationCodeAsync(
            Account account,
            string code,
            DateTime expiresAt);

        bool ValidEmailFormat(string email);

        bool PasswordFormat(string password);

        Task VerifyEmailAsync(Account account);

        // PASSWORD RESET
        Task SetPasswordResetCodeAsync(
            Account account,
            string code,
            DateTime expiresAt);

        Task ResetPasswordAsync(
            Account account,
            string newPassword);
    }
}
