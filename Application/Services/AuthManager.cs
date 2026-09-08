using ApplicationServices.DTOs.Account;
using ApplicationServices.DTOs.ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Domain.Entities;
using System.Security.Cryptography;


namespace ApplicationServices.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IEmployeeManager _employeeManager;
        private readonly IAccountManager _accountManager;
        private readonly IEmailService _emailService;

        public AuthManager(
            IAccountManager accountManager,
            IEmployeeManager employeeManager,
            IEmailService emailService)
        {
            _accountManager = accountManager;
            _employeeManager = employeeManager;
            _emailService = emailService;
        }

        // =========================================================
        // SIGN UP
        // =========================================================

        public async Task<AccountResponse> SignUp(SignUpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // =====================================================
            // REQUIRED FIELDS
            // =====================================================

            if (string.IsNullOrWhiteSpace(request.FName))
                throw new ArgumentException(
                    ErrorShared.Employee.FirstNameRequired);

            if (string.IsNullOrWhiteSpace(request.LName))
                throw new ArgumentException(
                    ErrorShared.Employee.LastNameRequired);

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new ArgumentException(
                    ErrorShared.Employee.PhoneRequired);

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException(
                    ErrorShared.Account.PasswordRequired);

            if (request.Password != request.ConfirmPassword)
                throw new ArgumentException(
                    ErrorShared.Account.PasswordsDoNotMatch);

            if (!request.AcceptTerms)
                throw new ArgumentException(
                    ErrorShared.Account.TermsNotAccepted);

            // =====================================================
            // FORMAT VALIDATION
            // =====================================================

            if (!_accountManager.ValidEmailFormat(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidEmail);

            if (!_accountManager.PasswordFormat(request.Password))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidPassword);

            if (!_employeeManager.ValidPhoneNumberFormat(request.Phone))
                throw new ArgumentException(
                    ErrorShared.Employee.InvalidPhoneNumber);

            // =====================================================
            // CHECK DUPLICATES
            // =====================================================

            if (await _accountManager.ExistsAsync(request.Email))
                throw new InvalidOperationException(
                    ErrorShared.Account.EmailAlreadyExists);

            if (await _employeeManager.ExistsByPhoneAsync(request.Phone))
                throw new InvalidOperationException(
                    ErrorShared.Employee.PhoneAlreadyExists);

            // =====================================================
            // CREATE EMPLOYEE
            // =====================================================

            var employee = Employee.Create(
                request.FName,
                request.LName,
                request.Phone,
                request.Gender);

            // =====================================================
            // GENERATE VERIFICATION CODE
            // =====================================================

            var verificationCode = GenerateVerificationCode();

            var verificationCodeExpiresAt =
                DateTime.UtcNow.AddMinutes(10);

            // =====================================================
            // CREATE ACCOUNT
            // =====================================================

            var account = Account.Create(
                request.Email,
                BCrypt.Net.BCrypt.HashPassword(request.Password),
                employee);

            account.SetVerificationCode(
                verificationCode,
                verificationCodeExpiresAt);

            // =====================================================
            // SAVE EMPLOYEE + ACCOUNT
            // =====================================================

            await _employeeManager.AddAsync(employee);

            // =====================================================
            // SEND VERIFICATION EMAIL
            // =====================================================

            await _emailService.SendVerificationCodeAsync(
                account.Email,
                verificationCode);

            // =====================================================
            // RESPONSE
            // =====================================================

            return new AccountResponse
            {
                Id = account.Id,
                Email = account.Email,
                EmployeeId = employee.Id
            };
        }

        // =========================================================
        // LOGIN
        // =========================================================

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException(
                    ErrorShared.Account.PasswordRequired);

            // =====================================================
            // GET ACCOUNT
            // =====================================================

            var account =
                await _accountManager
                    .GetEntityByEmailAsync(request.Email);

            // =====================================================
            // ACCOUNT DOESN'T EXIST
            // =====================================================

            if (account == null)
                throw new UnauthorizedAccessException(
                    ErrorShared.Account.InvalidCredentials);

            // =====================================================
            // CHECK ACCOUNT STATUS
            // =====================================================

            if (account.IsDeleted)
                throw new UnauthorizedAccessException(
                    ErrorShared.Account.AccountDeactivated);

            // =====================================================
            // CHECK EMPLOYEE STATUS
            // =====================================================

            if (account.Employee == null ||
                account.Employee.IsDeleted)
            {
                throw new UnauthorizedAccessException(
                    ErrorShared.Account.AccountDeactivated);
            }

            // =====================================================
            // CHECK EMAIL VERIFICATION
            // =====================================================

            if (!account.IsEmailVerified)
                throw new UnauthorizedAccessException(
                    ErrorShared.Account.EmailNotVerified);

            // =====================================================
            // CHECK PASSWORD
            // =====================================================

            if (!BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    account.PasswordHash))
            {
                throw new UnauthorizedAccessException(
                    ErrorShared.Account.InvalidCredentials);
            }

            // =====================================================
            // LOGIN RESPONSE
            // =====================================================

            return new LoginResponse
            {
                EmployeeId = account.EmployeeId,
                Email = account.Email,
                FName = account.Employee.FName,
                LName = account.Employee.LName
            };
        }

        // =========================================================
        // FORGOT PASSWORD
        // =========================================================

        public async Task ForgotPassword(ForgotPasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

            // =====================================================
            // VALIDATE EMAIL FORMAT
            // =====================================================

            if (!_accountManager.ValidEmailFormat(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidEmail);

            // =====================================================
            // GET ACCOUNT
            // =====================================================

            var account =
                await _accountManager
                    .GetEntityByEmailAsync(request.Email);

            if (account == null)
                throw new KeyNotFoundException(
                    ErrorShared.Account.AccountNotFound);

            // =====================================================
            // CHECK ACCOUNT STATUS
            // =====================================================

            if (account.IsDeleted)
                throw new InvalidOperationException(
                    ErrorShared.Account.AccountDeactivated);

            // =====================================================
            // GENERATE RESET CODE
            // =====================================================

            var resetCode = GenerateVerificationCode();

            var resetCodeExpiresAt =
                DateTime.UtcNow.AddMinutes(10);

            // =====================================================
            // SAVE RESET CODE
            // =====================================================

            await _accountManager.SetPasswordResetCodeAsync(
                account,
                resetCode,
                resetCodeExpiresAt);

            // =====================================================
            // SEND RESET CODE
            // =====================================================

            await _emailService.SendVerificationCodeAsync(
                account.Email,
                resetCode);
        }

        // =========================================================
        // RESET PASSWORD
        // =========================================================

        public async Task ResetPassword(ResetPasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // =====================================================
            // REQUIRED FIELDS
            // =====================================================

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException(
                    ErrorShared.Account.ResetCodeRequired);

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                throw new ArgumentException(
                    ErrorShared.Account.PasswordRequired);

            // =====================================================
            // VALIDATE EMAIL
            // =====================================================

            if (!_accountManager.ValidEmailFormat(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidEmail);

            // =====================================================
            // VALIDATE PASSWORD
            // =====================================================

            if (!_accountManager.PasswordFormat(request.NewPassword))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidPassword);

            // =====================================================
            // CHECK PASSWORD CONFIRMATION
            // =====================================================

            if (request.NewPassword != request.ConfirmPassword)
                throw new ArgumentException(
                    ErrorShared.Account.PasswordsDoNotMatch);

            // =====================================================
            // GET ACCOUNT
            // =====================================================

            var account =
                await _accountManager
                    .GetEntityByEmailAsync(request.Email);

            if (account == null)
                throw new KeyNotFoundException(
                    ErrorShared.Account.AccountNotFound);

            // =====================================================
            // CHECK ACCOUNT STATUS
            // =====================================================

            if (account.IsDeleted)
                throw new InvalidOperationException(
                    ErrorShared.Account.AccountDeactivated);

            // =====================================================
            // CHECK RESET CODE
            // =====================================================

            EnsureValidResetCode(account, request.Code);

            // =====================================================
            // RESET PASSWORD
            // =====================================================

            await _accountManager.ResetPasswordAsync(
                account,
                request.NewPassword);
        }

        public async Task VerifyEmail(VerifyEmailRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException(
                    ErrorShared.Account.VerificationCodeRequired);

            var account =
                await _accountManager
                    .GetEntityByEmailAsync(request.Email);

            if (account == null)
                throw new KeyNotFoundException(
                    ErrorShared.Account.AccountNotFound);

            if (account.IsEmailVerified)
                throw new InvalidOperationException(
                    ErrorShared.Account.EmailAlreadyVerified);

            if (string.IsNullOrWhiteSpace(account.VerificationCode))
                throw new InvalidOperationException(
                    ErrorShared.Account.InvalidVerificationCode);

            if (account.VerificationCode != request.Code)
                throw new InvalidOperationException(
                    ErrorShared.Account.InvalidVerificationCode);

            if (!account.VerificationCodeExpiresAt.HasValue)
                throw new InvalidOperationException(
                    ErrorShared.Account.VerificationCodeExpired);

            if (account.VerificationCodeExpiresAt.Value < DateTime.UtcNow)
                throw new InvalidOperationException(
                    ErrorShared.Account.VerificationCodeExpired);

            await _accountManager.VerifyEmailAsync(account);
        }

        public async Task ResendVerificationCode(ForgotPasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

            if (!_accountManager.ValidEmailFormat(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidEmail);

            var account =
                await _accountManager
                    .GetEntityByEmailAsync(request.Email);

            if (account == null)
                throw new KeyNotFoundException(
                    ErrorShared.Account.AccountNotFound);

            if (account.IsDeleted)
                throw new InvalidOperationException(
                    ErrorShared.Account.AccountDeactivated);

            if (account.IsEmailVerified)
                throw new InvalidOperationException(
                    ErrorShared.Account.EmailAlreadyVerified);

            var verificationCode = GenerateVerificationCode();
            var verificationCodeExpiresAt =
                DateTime.UtcNow.AddMinutes(10);

            await _accountManager.SetVerificationCodeAsync(
                account,
                verificationCode,
                verificationCodeExpiresAt);

            await _emailService.SendVerificationCodeAsync(
                account.Email,
                verificationCode);
        }

        public async Task VerifyResetCode(VerifyResetCodeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException(
                    ErrorShared.Account.ResetCodeRequired);

            if (!_accountManager.ValidEmailFormat(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidEmail);

            var account =
                await _accountManager
                    .GetEntityByEmailAsync(request.Email);

            if (account == null)
                throw new KeyNotFoundException(
                    ErrorShared.Account.AccountNotFound);

            if (account.IsDeleted)
                throw new InvalidOperationException(
                    ErrorShared.Account.AccountDeactivated);

            EnsureValidResetCode(account, request.Code);
        }

        private static void EnsureValidResetCode(Account account, string code)
        {
            if (string.IsNullOrWhiteSpace(account.PasswordResetCode))
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.InvalidResetCode);
            }

            if (account.PasswordResetCode != code)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.InvalidResetCode);
            }

            if (!account.PasswordResetCodeExpiresAt.HasValue)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.ResetCodeExpired);
            }

            if (account.PasswordResetCodeExpiresAt.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.ResetCodeExpired);
            }
        }

        private static string GenerateVerificationCode()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();
        }
    }
}
