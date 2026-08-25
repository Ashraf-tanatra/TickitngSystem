using ApplicationServices.DTOs.Account;
using ApplicationServices.DTOs.ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IEmployeeManager _employeeManager;
        private readonly IAccountManager _accountManager;

        public AuthManager(
            IAccountManager accountManager,
            IEmployeeManager employeeManager)
        {
            _accountManager = accountManager;
            _employeeManager = employeeManager;
        }

        // =========================================================
        // SIGN UP
        // =========================================================

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

            // =====================================================
            // FORMAT VALIDATION
            // =====================================================

            if (!_accountManager.ValidEmailFormat(request.Email))
                throw new ArgumentException(
                    "Invalid email format.");

            if (!_accountManager.PasswordFormat(request.Password))
                throw new ArgumentException(
                    "Password must be at least 8 characters and contain " +
                    "uppercase, lowercase, number, and special character.");

            if (!_employeeManager.ValidPhoneNumberFormat(request.Phone))
                throw new ArgumentException(
                    "Phone number must contain exactly 10 digits.");

            // =====================================================
            // CHECK DUPLICATES
            // =====================================================

            if (await _accountManager.ExistsAsync(request.Email))
                throw new InvalidOperationException(
                    "An account with this email already exists.");

            // NOTE:
            // ExistsByPhone is still synchronous in your current
            // IEmployeeManager, so it remains synchronous here.
            if (await _employeeManager.ExistsByPhoneAsync(request.Phone))
                throw new InvalidOperationException(
                    "An employee with this phone already exists.");

            // =====================================================
            // CREATE EMPLOYEE
            // =====================================================

            var employee = new Employee
            {
                FName = request.FName,
                LName = request.LName,
                Phone = request.Phone,
                Gender = request.Gender,
                IsDeleted = false
            };

            // =====================================================
            // CREATE ACCOUNT
            // =====================================================

            var account = new Account
            {
                Email = request.Email,
                PasswordHash = request.Password,
                Employee = employee
            };

            employee.Account = account;

            // =====================================================
            // SAVE EMPLOYEE + ACCOUNT
            // =====================================================

            // This remains synchronous until EmployeeManager/Add
            // is also converted to async.
            _employeeManager.AddAsync(employee);

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

        public async Task<LoginResponse> Login(
            LoginRequest request)
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
                await _accountManager
                    .GetEntityByEmailAsync(request.Email);

            // =====================================================
            // ACCOUNT DOESN'T EXIST
            // =====================================================

            if (account == null)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");

            // =====================================================
            // CHECK EMPLOYEE / ACCOUNT STATUS
            // =====================================================

            if (account.Employee == null ||
                account.Employee.IsDeleted)
            {
                throw new UnauthorizedAccessException(
                    "This account is deactivated.");
            }

            // =====================================================
            // CHECK PASSWORD
            // =====================================================

            // TEMPORARY
            // Replace with password hashing later.
            if (account.PasswordHash != request.Password)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");

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
    }
}