using ApplicationServices.DTOs.Account;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;
using System.Net.Mail;

namespace ApplicationServices.Services
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public AccountManager(
            IAccountRepository accountRepository,
            IEmployeeRepository employeeRepository)
        {
            _accountRepository = accountRepository;
            _employeeRepository = employeeRepository;
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
                    ErrorShared.Account.EmailRequired);

            if (!ValidEmailFormat(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidEmail);

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException(
                    ErrorShared.Account.PasswordRequired);

            if (!PasswordFormat(request.Password))
                throw new ArgumentException(
                    ErrorShared.Account.InvalidPassword);

            // Check duplicate email
            if (await _accountRepository
                .GetByEmailAsync(request.Email) != null)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.EmailAlreadyExists);
            }

            throw new InvalidOperationException(
                ErrorShared.Account.CreateAccountThroughSignup);
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
                    ErrorShared.Account.EmailRequired);

            var account =
                await _accountRepository.GetByEmailAsync(email);

            if (account == null)
                return null;

            return MapToResponse(account);
        }

        // =========================================================
        // GET ENTITY BY EMAIL
        // =========================================================

        public async Task<Account?> GetEntityByEmailAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

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
                    ErrorShared.Account.CannotUpdateDeletedAccount);
            }

            // Current password is required
            if (string.IsNullOrWhiteSpace(
                request.CurrentPassword))
            {
                throw new ArgumentException(
                    ErrorShared.Account.CurrentPasswordRequired);
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.PasswordHash))
            {
                throw new UnauthorizedAccessException(
                    ErrorShared.Account.CurrentPasswordIncorrect);
            }

            // =====================================================
            // UPDATE EMAIL
            // =====================================================

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (!ValidEmailFormat(request.Email))
                {
                    throw new ArgumentException(
                        ErrorShared.Account.InvalidEmail);
                }

                if (request.Email != account.Email &&
                    await _accountRepository
                        .EmailExistsAsync(request.Email))
                {
                    throw new InvalidOperationException(
                        ErrorShared.Account.EmailAlreadyExists);
                }

                account.ChangeEmail(request.Email);
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
                        ErrorShared.Account.InvalidPassword);
                }

                if (request.NewPassword !=
                    request.ConfirmNewPassword)
                {
                    throw new ArgumentException(
                        ErrorShared.Account.PasswordsDoNotMatch);
                }

                account.ChangePassword(BCrypt.Net.BCrypt.HashPassword(request.NewPassword));
            }

            await _accountRepository.UpdateAsync(account);

            return MapToResponse(account);
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
                    ErrorShared.Account.EmailRequired);

            var account =
                await _accountRepository.GetByEmailAsync(email);

            if (account == null)
                return false;

            if (account.IsDeleted)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.AccountAlreadyDeleted);
            }

            await DeactivateAccountAsync(account);

            return true;
        }

        public async Task<bool> SoftDeleteAsync(
            DeactivateAccountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    ErrorShared.Account.EmailRequired);

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                throw new ArgumentException(
                    ErrorShared.Account.CurrentPasswordRequired);

            var account =
                await _accountRepository.GetByEmailAsync(request.Email);

            if (account == null)
                return false;

            if (account.IsDeleted)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.AccountAlreadyDeleted);
            }

            if (!BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                account.PasswordHash))
            {
                throw new UnauthorizedAccessException(
                    ErrorShared.Account.CurrentPasswordIncorrect);
            }

            await DeactivateAccountAsync(account);

            return true;
        }

        private async Task DeactivateAccountAsync(Account account)
        {
            var employee = account.Employee;
            var managedProjects = await _employeeRepository
                .GetActiveProjectsAsync(employee.Id);

            if (managedProjects.Any())
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.CannotDeactivateProjectManager);
            }

            var activeTickets = (await _employeeRepository
                    .GetEmployeeTicketsAsync(employee.Id))
                .Where(ticket =>
                    ticket.TicketStatus != TicketStatus.Done &&
                    ticket.TicketStatus != TicketStatus.Completed &&
                    ticket.TicketStatus != TicketStatus.Cancelled)
                .ToList();

            var histories = new List<TicketHistory>(activeTickets.Count);

            foreach (var ticket in activeTickets)
            {
                histories.Add(TicketHistory.Create(
                    ticket.TicketId,
                    employee.Id,
                    ErrorShared.Ticket.UnassignedDueToAccountDeactivationAction,
                    oldValue: ticket.TicketStatus.ToString(),
                    newValue: TicketStatus.Pending.ToString(),
                    fromEmployeeId: employee.Id,
                    note: ErrorShared.Ticket.UnassignedDueToAccountDeactivationNote));

                ticket.UnassignAndResetToPending();
            }

            account.Deactivate();

            if (!employee.IsDeleted)
                employee.Deactivate();

            await _accountRepository.SaveDeactivationAsync(
                account,
                activeTickets,
                histories);
        }

        public async Task SetVerificationCodeAsync(Account account,string code,DateTime expiresAt)
        {
            account.SetVerificationCode(code, expiresAt);

            await _accountRepository.UpdateAsync(account);
        }

        public async Task VerifyEmailAsync(Account account)
        {
            account.VerifyEmail();

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
                    ErrorShared.Account.EmailRequired);

            var account = await _accountRepository.GetByEmailAsync(email);

            if (account == null)
                return false;

            if (!account.IsDeleted)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.AccountAlreadyActive);
            }

            if (!account.DeletedAt.HasValue)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.AccountDeletionDateMissing);
            }

            if (account.DeletedAt.Value.AddDays(ErrorShared.Account.ReactivationPeriodDays)
                < DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    ErrorShared.Account.ReactivationPeriodExpired);
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
            account.ChangePassword(
                BCrypt.Net.BCrypt.HashPassword(newPassword));

            await _accountRepository.UpdateAsync(account);
        }// =========================================================
         // SET PASSWORD RESET CODE
         // =========================================================

        public async Task SetPasswordResetCodeAsync(
            Account account,
            string code,
            DateTime expiresAt)
        {
            account.SetPasswordResetCode(code, expiresAt);

            await _accountRepository.UpdateAsync(account);
        }

        private static AccountResponse MapToResponse(Account account)
        {
            return new AccountResponse
            {
                Id = account.Id,
                Email = account.Email,
                EmployeeId = account.EmployeeId
            };
        }
    }
}
