using ApplicationServices.DTOs.Account;
using ApplicationServices.Interfaces;
using Domain.Interfaces;

namespace ApplicationServices.Services
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

        public AccountResponse? GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            var account = _accountRepository.GetByEmail(email);

            if (account == null)
                return null;

            return new AccountResponse
            {
                Email = account.Email
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