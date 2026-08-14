using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
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

        public LoginResponse Login(LoginRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");

            var account = _accountRepository.GetByEmail(request.Email);

            if (account == null)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");

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