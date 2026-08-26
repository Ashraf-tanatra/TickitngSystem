using ApplicationServices.DTOs.Account;
//using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IAccountManager
    {
        Task<bool> ExistsAsync(string email);
        Task<bool> DeleteAsync(string email);
        Task<bool> ReactivateAsync(string email);
        Task<bool> SoftDeleteAsync(string email);
        Task<Account?> GetEntityByEmailAsync(string email);
        Task<AccountResponse?> GetByEmailAsync(string email);
        Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request);
        Task<AccountResponse?> UpdateAsync(int id, UpdateAccountRequest request);

        // EMAIL VERIFICATION
        bool ValidEmailFormat(string email);
        bool PasswordFormat(string password);
        Task VerifyEmailAsync(Account account);
        Task SetVerificationCodeAsync(Account account, string code, DateTime expiresAt);

        // PASSWORD RESET
        Task ResetPasswordAsync(Account account, string newPassword);
        Task SetPasswordResetCodeAsync(Account account, string code, DateTime expiresAt);
    }
}