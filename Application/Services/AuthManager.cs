using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IAccountRepository _accountRepository;

        public AuthManager(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // SIGN UP
        public AccountResponse SignUp(SignUpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    "Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException(
                    "Password is required.");

            if (_accountRepository.EmailExists(request.Email))
                throw new ArgumentException(
                    "Email already exists.");

            if (!_accountRepository.EmployeeExists(request.EmployeeId))
                throw new ArgumentException(
                    "Employee does not exist.");

            var account = new Account
            {
                Email = request.Email,

                // TEMPORARY
                // Later replace with real password hashing.
                PasswordHash = request.Password,

                EmployeeId = request.EmployeeId
            };

            _accountRepository.Add(account);

            return new AccountResponse
            {
                Id = account.Id,
                Email = account.Email,
                EmployeeId = account.EmployeeId
            };
        }

        // LOGIN
        public LoginResponse Login(LoginRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    "Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException(
                    "Password is required.");

            var account =
                _accountRepository.GetByEmail(request.Email);

            if (account == null)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");

            // TEMPORARY
            // Later replace with password hash verification.
            if (account.PasswordHash != request.Password)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");

            return new LoginResponse
            {
                EmployeeId = account.EmployeeId,
                Email = account.Email,
                Role = account.Employee.Role.ToString()
            };
        }
    }
}