namespace Domain.Entities
{
    public class Account : BaseEntity
    {
        private Account()
        {
        }

        public string Email { get; private set; } = string.Empty;
        public int EmployeeId { get; private set; }
        public Employee Employee { get; private set; } = null!;
        public DateTime? DeletedAt { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;
        public bool IsDeleted { get; private set; }

        //From Verify Account
        public bool IsEmailVerified { get; private set; }
        public string? VerificationCode { get; private set; }
        public DateTime? VerificationCodeExpiresAt { get; private set; }

        //For Forget Password
        public string? PasswordResetCode { get; private set; }
        public DateTime? PasswordResetCodeExpiresAt { get; private set; }

        public static Account Create(
            string email,
            string passwordHash,
            Employee employee)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(ErrorShared.Account.EmailRequired);

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(ErrorShared.Account.PasswordRequired);

            ArgumentNullException.ThrowIfNull(employee);

            var account = new Account
            {
                Email = email.Trim(),
                PasswordHash = passwordHash,
                Employee = employee,
                IsDeleted = false,
                IsEmailVerified = false
            };

            employee.AttachAccount(account);
            return account;
        }

        public static Account Create(
            string email,
            string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(ErrorShared.Account.EmailRequired);

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(ErrorShared.Account.PasswordRequired);

            return new Account
            {
                Email = email.Trim(),
                PasswordHash = passwordHash,
                IsDeleted = false,
                IsEmailVerified = false
            };
        }

        public void ChangeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(ErrorShared.Account.EmailRequired);

            Email = email.Trim();
            Touch();
        }

        public void ChangePassword(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(ErrorShared.Account.PasswordRequired);

            PasswordHash = passwordHash;
            ClearPasswordResetCode();
            Touch();
        }

        public void SetVerificationCode(string code, DateTime expiresAt)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException(ErrorShared.Account.VerificationCodeRequired);

            VerificationCode = code;
            VerificationCodeExpiresAt = expiresAt;
            IsEmailVerified = false;
            Touch();
        }

        public void VerifyEmail()
        {
            IsEmailVerified = true;
            VerificationCode = null;
            VerificationCodeExpiresAt = null;
            Touch();
        }

        public void SetPasswordResetCode(string code, DateTime expiresAt)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException(ErrorShared.Account.ResetCodeRequired);

            PasswordResetCode = code;
            PasswordResetCodeExpiresAt = expiresAt;
            Touch();
        }

        public void ClearPasswordResetCode()
        {
            PasswordResetCode = null;
            PasswordResetCodeExpiresAt = null;
        }

        public void Deactivate()
        {
            if (IsDeleted)
                throw new InvalidOperationException(ErrorShared.Account.AccountAlreadyDeleted);

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            Touch();
        }

        public void Reactivate()
        {
            if (!IsDeleted)
                throw new InvalidOperationException(ErrorShared.Account.AccountAlreadyActive);

            IsDeleted = false;
            DeletedAt = null;
            Touch();
        }
    }
}
