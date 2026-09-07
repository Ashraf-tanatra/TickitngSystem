using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET ACCOUNT BY EMAIL
        // =========================================================

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        // =========================================================
        // GET ACCOUNT BY ID
        // =========================================================

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // =========================================================
        // CHECK EMAIL
        // =========================================================

        public async Task<bool> EmailExistsAsync(string email)
        {
            // Deleted accounts are still considered existing
            // because they can be reactivated within 30 days.
            return await _context.Accounts
                .AnyAsync(a => a.Email == email);
        }

        // =========================================================
        // CHECK EMPLOYEE
        // =========================================================

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.Id == employeeId);
        }

        // =========================================================
        // ADD ACCOUNT
        // =========================================================

        public async Task AddAsync(Account account)
        {
            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE ACCOUNT
        // =========================================================

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // HARD DELETE
        // =========================================================

        public async Task DeleteAsync(Account account)
        {
            _context.Accounts.Remove(account);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // SOFT DELETE
        // =========================================================

        public async Task SoftDeleteAsync(Account account)
        {
            account.Deactivate();

            _context.Accounts.Update(account);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // REACTIVATE
        // =========================================================

        public async Task ReactivateAsync(Account account)
        {
            account.Reactivate();

            _context.Accounts.Update(account);

            await _context.SaveChangesAsync();
        }
    }
}
