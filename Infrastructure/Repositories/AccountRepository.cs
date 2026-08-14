using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;

namespace Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public Account? GetById(int id)
        {
            return _context.Accounts.FirstOrDefault(a => a.Id == id);
        }

        public Account? GetByEmail(string email)
        {
            return _context.Accounts.FirstOrDefault(a => a.Email == email);
        }

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void Delete(Account account)
        {
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }
    }
}