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


        // GET ACCOUNT BY EMAIL
        public Account? GetByEmail(string email)
        {
            return _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefault(a => a.Email == email);
        }


        // CHECK EMAIL
        public bool EmailExists(string email)
        {
            return _context.Accounts
                .Any(a => a.Email == email);
        }


        // CHECK EMPLOYEE
        public bool EmployeeExists(int employeeId)
        {
            return _context.Employees
                .Any(e => e.Id == employeeId);
        }


        // ADD ACCOUNT
        public void Add(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }


        // UPDATE ACCOUNT
        public void Update(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }


        // HARD DELETE
        public void Delete(Account account)
        {
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }
    }
}