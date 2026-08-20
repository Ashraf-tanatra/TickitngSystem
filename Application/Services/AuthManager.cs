using ApplicationServices.DTOs;
using ApplicationServices.DTOs.ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public AuthManager(
            IAccountRepository accountRepository,
            IEmployeeRepository employeeRepository,
            IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
        }


        // SIGN UP
        public async Task<AccountResponse> SignUp(
            SignUpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.FName))
                throw new ArgumentException(
                    "First name is required.");

            if (string.IsNullOrWhiteSpace(request.LName))
                throw new ArgumentException(
                    "Last name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    "Email is required.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new ArgumentException(
                    "Phone is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException(
                    "Password is required.");

            if (request.Password != request.ConfirmPassword)
                throw new ArgumentException(
                    "Password and confirm password do not match.");

            if (!request.AcceptTerms)
                throw new ArgumentException(
                    "You must accept the Terms of Service and Privacy Policy.");


            // Check Email
            if (_accountRepository.EmailExists(request.Email))
                throw new InvalidOperationException(
                    "An account with this email already exists.");


            // Check Phone
            if (_employeeRepository.ExistsByPhone(request.Phone))
                throw new InvalidOperationException(
                    "An employee with this phone already exists.");


            // Create Employee
            var employee = new Employee
            {
                FName = request.FName,
                LName = request.LName,
                Phone = request.Phone,
                Gender = request.Gender
            };


            // Generate Verification Code
            var verificationCode = Random.Shared
                .Next(100000, 1000000)
                .ToString();


            // Create Account
            var account = new Account
            {
                Email = request.Email,

                // TEMPORARY
                // Later replace with real password hashing.
                PasswordHash = request.Password,

                VerificationCode = verificationCode,

                VerificationCodeExpiresAt =
                    DateTime.Now.AddMinutes(10),

                EmailConfirmed = false,

                Employee = employee
            };


            employee.Account = account;


            // Save Employee + Account
            _employeeRepository.Add(employee);


            // Send Verification Email
            await _emailService.SendVerificationCodeAsync(
                account.Email!,
                verificationCode);


            return new AccountResponse
            {
                Id = account.Id,
                Email = account.Email,
                EmployeeId = employee.Id
            };
        }


        // VERIFY EMAIL
        public void VerifyEmail(
            VerifyEmailRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    "Email is required.");

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException(
                    "Verification code is required.");


            var account =
                _accountRepository.GetByEmail(request.Email);


            if (account == null)
                throw new KeyNotFoundException(
                    "Account not found.");


            if (account.EmailConfirmed)
                throw new InvalidOperationException(
                    "Email is already verified.");


            if (account.VerificationCode != request.Code)
                throw new ArgumentException(
                    "Invalid verification code.");


            if (account.VerificationCodeExpiresAt == null ||
                account.VerificationCodeExpiresAt < DateTime.Now)
            {
                throw new InvalidOperationException(
                    "Verification code has expired.");
            }


            // Confirm Email
            account.EmailConfirmed = true;

            // Remove used code
            account.VerificationCode = null;

            account.VerificationCodeExpiresAt = null;


            _accountRepository.Update(account);
        }


        // RESEND VERIFICATION CODE
        public async Task ResendVerificationCode(
            ResendVerificationCodeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    "Email is required.");


            var account =
                _accountRepository.GetByEmail(request.Email);


            if (account == null)
                throw new KeyNotFoundException(
                    "Account not found.");


            if (account.EmailConfirmed)
                throw new InvalidOperationException(
                    "Email is already verified.");


            // Generate new code
            var verificationCode = Random.Shared
                .Next(100000, 1000000)
                .ToString();


            account.VerificationCode =
                verificationCode;

            account.VerificationCodeExpiresAt =
                DateTime.Now.AddMinutes(10);


            _accountRepository.Update(account);


            // Send new code
            await _emailService.SendVerificationCodeAsync(
                account.Email!,
                verificationCode);
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


            if (account.IsDeleted ||
                account.Employee.IsDeleted)
            {
                throw new UnauthorizedAccessException(
                    "This account is deactivated.");
            }


            // Email must be verified
            if (!account.EmailConfirmed)
            {
                throw new UnauthorizedAccessException(
                    "Please verify your email before logging in.");
            }


            // TEMPORARY
            // Later replace with real password hashing.
            if (account.PasswordHash != request.Password)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");


            return new LoginResponse
            {
                EmployeeId = account.EmployeeId,
                Email = account.Email,
                FName = account.Employee.FName,
                LName = account.Employee.LName
            };
        }

       
    }
}