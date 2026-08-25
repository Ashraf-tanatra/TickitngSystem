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
            Console.WriteLine(
                "1 - Before query ===============================>");

            var account = await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Email == email);

            Console.WriteLine(
                "2 - After query ===============================>");

            return account;
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
            account.IsDeleted = true;
            account.DeletedAt = DateTime.Now;

            _context.Accounts.Update(account);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // REACTIVATE
        // =========================================================

        public async Task ReactivateAsync(Account account)
        {
            account.IsDeleted = false;
            account.DeletedAt = null;

            _context.Accounts.Update(account);

            await _context.SaveChangesAsync();
        }
    }
}