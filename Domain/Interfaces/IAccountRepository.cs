using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByEmailAsync(string email);
        Task<Account?> GetByIdAsync(int id);

        Task<bool> EmailExistsAsync(string email);
        Task<bool> EmployeeExistsAsync(int employeeId);

        Task AddAsync(Account account);
        Task UpdateAsync(Account account);

        Task DeleteAsync(Account account);
        Task SoftDeleteAsync(Account account);
        Task ReactivateAsync(Account account);
        
    }
}
