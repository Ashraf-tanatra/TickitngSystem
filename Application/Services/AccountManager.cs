using ApplicationServices.DTOs.Account;
using ApplicationServices.Interfaces;
using Domain.Interfaces;
using System.Net.Mail;

namespace ApplicationServices.Services
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountRepository _accountRepository;

        public AccountManager(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // =========================================================
        // CREATE ACCOUNT
        // =========================================================
        public AccountResponse CreateAccount(CreateAccountRequest request)
        {
            // Validate request
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            if (!ValidEmailFormat(request.Email))
                throw new ArgumentException("Invalid email format.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");

            if (!PasswordFormat(request.Password))
            {
                throw new ArgumentException(
                    "Password must be at least 8 characters and contain " +
                    "uppercase, lowercase, number, and special character.");
            }

            // Check duplicate email
            if (_accountRepository.GetByEmail(request.Email) != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            // Create account
            var account = new Account
            {
                Email = request.Email,
                PasswordHash = request.Password
            };

            // Save
            _accountRepository.Add(account);

            // Return response
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
        public AccountResponse? GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            var account = _accountRepository.GetByEmail(email);

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
        public Account? GetEntityByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            return _accountRepository.GetByEmail(email);
        }


        // =========================================================
        // UPDATE ACCOUNT
        // =========================================================
        public AccountResponse? Update(
            int id,
            UpdateAccountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var account = _accountRepository.GetById(id);

            if (account == null)
                return null;

            // Check account status
            if (account.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Cannot update a deleted account.");
            }

            // Current password is required
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                throw new ArgumentException(
                    "Current password is required.");
            }

            // Check current password
            if (account.PasswordHash != request.CurrentPassword)
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
                    _accountRepository.EmailExists(request.Email))
                {
                    throw new InvalidOperationException(
                        "An account with this email already exists.");
                }

                account.Email = request.Email;
            }

            // =====================================================
            // UPDATE PASSWORD
            // =====================================================
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                if (!PasswordFormat(request.NewPassword))
                {
                    throw new ArgumentException(
                        "Password must be at least 8 characters and contain " +
                        "uppercase, lowercase, number, and special character.");
                }

                if (request.NewPassword != request.ConfirmNewPassword)
                {
                    throw new ArgumentException(
                        "New password and confirm password do not match.");
                }

                account.PasswordHash = request.NewPassword;
            }

            // Save changes
            _accountRepository.Update(account);

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
        public bool Exists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return _accountRepository.GetByEmail(email) != null;
        }


        // =========================================================
        // HARD DELETE
        // =========================================================
        public bool Delete(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var account = _accountRepository.GetByEmail(email);

            if (account == null)
                return false;

            _accountRepository.Delete(account);

            return true;
        }


        // =========================================================
        // SOFT DELETE
        // =========================================================
        public bool SoftDelete(string email)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            // Find account
            var account = _accountRepository.GetByEmail(email);

            if (account == null)
                return false;

            // Check if already deleted
            if (account.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Account is already deleted.");
            }

            // Soft delete
            _accountRepository.SoftDelete(account);

            return true;
        }


        // =========================================================
        // REACTIVATE ACCOUNT
        // =========================================================
        public bool Reactivate(string email)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            // Get account
            var account = _accountRepository.GetByEmail(email);

            if (account == null)
                return false;

            // Check if account is already active
            if (!account.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Account is already active.");
            }

            // DeletedAt must exist
            if (!account.DeletedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Account deletion date is missing.");
            }

            // Check 30-day reactivation period
            if (account.DeletedAt.Value.AddDays(30) < DateTime.Now)
            {
                throw new InvalidOperationException(
                    "The 30-day reactivation period has expired.");
            }

            // Reactivate account
            _accountRepository.Reactivate(account);

            return true;
        }
    }
}