using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Net.Mail;

namespace ApplicationServices.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountRepository _accountRepository;

        public AccountManager(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public AccountResponse CreateAccount(CreateAccountRequest request)
        {
            // 1. Validate request
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");

            // 2. Check duplicate email
            if (_accountRepository.GetByEmail(request.Email) != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            // 3. Create Account
            var account = new Account
            {
                Email = request.Email,
                PasswordHash = request.Password
            };

            // 4. Save
            _accountRepository.Add(account);

            // 5. Return response
            return new AccountResponse
            {
                Email = account.Email
            };
        }

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

        public AccountResponse? GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            var account = _accountRepository.GetByEmail(email);

            if (account == null)
                return null;

            return new AccountResponse
            {
                Id=account.Id,
                Email = account.Email,
                EmployeeId=account.EmployeeId
            };
        }

        // GET ENTITY BY EMAIL
        public Account? GetEntityByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            return _accountRepository.GetByEmail(email);
        }

        public AccountResponse? Update(
    int id,
    UpdateAccountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var account = _accountRepository.GetById(id);

            if (account == null)
                return null;

            if (account.Employee == null ||
                account.Employee.IsDeleted)
            {
                throw new InvalidOperationException(
                    "Cannot update a deactivated account.");
            }

            // Current password is required
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                throw new ArgumentException(
                    "Current password is required.");

            // Check current password
            if (account.PasswordHash != request.CurrentPassword)
                throw new UnauthorizedAccessException(
                    "Current password is incorrect.");

            // Update Email
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (!ValidEmailFormat(request.Email))
                    throw new ArgumentException(
                        "Invalid email format.");

                if (request.Email != account.Email &&
                    _accountRepository.EmailExists(request.Email))
                {
                    throw new InvalidOperationException(
                        "An account with this email already exists.");
                }

                account.Email = request.Email;
            }

            // Update Password
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                if (!PasswordFormat(request.NewPassword))
                    throw new ArgumentException(
                        "Password must be at least 8 characters and contain " +
                        "uppercase, lowercase, number, and special character.");

                if (request.NewPassword != request.ConfirmNewPassword)
                    throw new ArgumentException(
                        "New password and confirm password do not match.");

                account.PasswordHash = request.NewPassword;
            }

            _accountRepository.Update(account);

            return new AccountResponse
            {
                Id = account.Id,
                Email = account.Email,
                EmployeeId = account.EmployeeId
            };
        }
        public bool Exists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return _accountRepository.GetByEmail(email) != null;
        }

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
    }
}