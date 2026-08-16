using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
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

        public Account? GetByEmail(string email)
        {
            return _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefault(a => a.Email == email);
        }

        public bool EmailExists(string email)
        {
            return _context.Accounts
                .Any(a => a.Email == email);
        }

        public bool EmployeeExists(int employeeId)
        {
            return _context.Employees
                .Any(e => e.Id == employeeId);
        }

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public void Delete(Account account)
        {
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }
    }
}