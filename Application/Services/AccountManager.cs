using ApplicationServices.DTOs.Account;
using ApplicationServices.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;

namespace ApplicationServices.Services
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountRepository _accountRepository;

        public AccountManager(
            IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // =========================================================
        // CREATE ACCOUNT
        // =========================================================

        public async Task<AccountResponse> CreateAccountAsync(
            CreateAccountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    "Email is required.");

            if (!ValidEmailFormat(request.Email))
                throw new ArgumentException(
                    "Invalid email format.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException(
                    "Password is required.");

            if (!PasswordFormat(request.Password))
                throw new ArgumentException(
                    "Password must be at least 8 characters and contain " +
                    "uppercase, lowercase, number, and special character.");

            // Check duplicate email
            if (await _accountRepository
                .GetByEmailAsync(request.Email) != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            var account = new Account
            {
                Email = request.Email,
                PasswordHash = request.Password
            };

            await _accountRepository.AddAsync(account);

            return new AccountResponse
            {
                Id = account.Id,
                Email = account.Email,
                EmployeeId = account.EmployeeId
            };
        }

        // =========================================================
        // VALIDATE EMAIL
        // =========================================================

        public bool ValidEmailFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mail = new MailAddress(email);

                return mail.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // =========================================================
        // VALIDATE PASSWORD
        // =========================================================

        public bool PasswordFormat(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8)
                return false;

            if (!password.Any(char.IsUpper))
                return false;

            if (!password.Any(char.IsLower))
                return false;

            if (!password.Any(char.IsDigit))
                return false;

            if (!password.Any(c => "@#$!".Contains(c)))
                return false;

            return true;
        }

        // =========================================================
        // GET ACCOUNT BY EMAIL
        // =========================================================

        public async Task<AccountResponse?> GetByEmailAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email is required.");

            var account =
                await _accountRepository.GetByEmailAsync(email);

            if (account == null)
                return null;

            return new AccountResponse
            {
                Id = account.Id,
                Email = account.Email,
                EmployeeId = account.EmployeeId
            };
        }

        // =========================================================
        // GET ENTITY BY EMAIL
        // =========================================================

        public async Task<Account?> GetEntityByEmailAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email is required.");

            return await _accountRepository
                .GetByEmailAsync(email);
        }

        // =========================================================
        // UPDATE ACCOUNT
        // =========================================================

        public async Task<AccountResponse?> UpdateAsync(
            int id,
            UpdateAccountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var account =
                await _accountRepository.GetByIdAsync(id);

            if (account == null)
                return null;

            // Check account status
            if (account.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Cannot update a deleted account.");
            }

            // Current password is required
            if (string.IsNullOrWhiteSpace(
                request.CurrentPassword))
            {
                throw new ArgumentException(
                    "Current password is required.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword,account.PasswordHash))
            {
                throw new UnauthorizedAccessException(
                    "Current password is incorrect.");
            }

            // =====================================================
            // UPDATE EMAIL
            // =====================================================

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (!ValidEmailFormat(request.Email))
                {
                    throw new ArgumentException(
                        "Invalid email format.");
                }

                if (request.Email != account.Email &&
                    await _accountRepository
                        .EmailExistsAsync(request.Email))
                {
                    throw new InvalidOperationException(
                        "An account with this email already exists.");
                }

                account.Email = request.Email;
            }

            // =====================================================
            // UPDATE PASSWORD
            // =====================================================

            if (!string.IsNullOrWhiteSpace(
                request.NewPassword))
            {
                if (!PasswordFormat(
                    request.NewPassword))
                {
                    throw new ArgumentException(
                        "Password must be at least 8 characters and contain " +
                        "uppercase, lowercase, number, and special character.");
                }

                if (request.NewPassword !=
                    request.ConfirmNewPassword)
                {
                    throw new ArgumentException(
                        "New password and confirm password do not match.");
                }

                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            }

            await _accountRepository.UpdateAsync(account);

            return new AccountResponse
            {
                Id = account.Id,
                Email = account.Email,
                EmployeeId = account.EmployeeId
            };
        }

        // =========================================================
        // CHECK ACCOUNT EXISTS
        // =========================================================

        public async Task<bool> ExistsAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _accountRepository
                .GetByEmailAsync(email) != null;
        }

        // =========================================================
        // HARD DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var account =
                await _accountRepository.GetByEmailAsync(email);

            if (account == null)
                return false;

            await _accountRepository.DeleteAsync(account);

            return true;
        }

        // =========================================================
        // SOFT DELETE
        // =========================================================

        public async Task<bool> SoftDeleteAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email is required.");

            var account =
                await _accountRepository.GetByEmailAsync(email);

            if (account == null)
                return false;

            if (account.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Account is already deleted.");
            }

            await _accountRepository
                .SoftDeleteAsync(account);

            return true;
        }

        public async Task SetVerificationCodeAsync(Account account,string code,DateTime expiresAt)
        {
            account.VerificationCode = code;
            account.VerificationCodeExpiresAt = expiresAt;
            account.IsEmailVerified = false;

            await _accountRepository.UpdateAsync(account);
        }

        public async Task VerifyEmailAsync(Account account)
        {
            account.IsEmailVerified = true;
            account.VerificationCode = null;
            account.VerificationCodeExpiresAt = null;

            await _accountRepository.UpdateAsync(account);
        }

        // =========================================================
        // REACTIVATE ACCOUNT
        // =========================================================

        public async Task<bool> ReactivateAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email is required.");

            var account = await _accountRepository.GetByEmailAsync(email);

            if (account == null)
                return false;

            if (!account.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Account is already active.");
            }

            if (!account.DeletedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Account deletion date is missing.");
            }

            if (account.DeletedAt.Value.AddDays(30)
                < DateTime.Now)
            {
                throw new InvalidOperationException(
                    "The 30-day reactivation period has expired.");
            }


            await _accountRepository
                .ReactivateAsync(account);

            return true;
        }

        // =========================================================
        // RESET PASSWORD
        // =========================================================

        public async Task ResetPasswordAsync(
            Account account,
            string newPassword)
        {
            account.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(newPassword);

            account.PasswordResetCode = null;
            account.PasswordResetCodeExpiresAt = null;

            await _accountRepository.UpdateAsync(account);
        }// =========================================================
         // SET PASSWORD RESET CODE
         // =========================================================

        public async Task SetPasswordResetCodeAsync(
            Account account,
            string code,
            DateTime expiresAt)
        {
            account.PasswordResetCode = code;
            account.PasswordResetCodeExpiresAt = expiresAt;

            await _accountRepository.UpdateAsync(account);
        }
    }
}