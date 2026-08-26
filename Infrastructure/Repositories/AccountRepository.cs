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

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            // Deleted accounts are still considered existing
            // because they can be reactivated within 30 days.
            return await _context.Accounts
                .AnyAsync(a => a.Email == email);
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.Id == employeeId);
        }

        public async Task AddAsync(Account account)
        {
            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Account account)
        {
            _context.Accounts.Remove(account);

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Account account)
        {
            account.IsDeleted = true;
            account.DeletedAt = DateTime.UtcNow;

            _context.Accounts.Update(account);

            await _context.SaveChangesAsync();
        }

        public async Task ReactivateAsync(Account account)
        {
            account.IsDeleted = false;
            account.DeletedAt = null;

            _context.Accounts.Update(account);

            await _context.SaveChangesAsync();
        }
    }
}