namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);
        Task<Account?> GetByIdAsync(int id);
        Task ReactivateAsync(Account account);
        Task SoftDeleteAsync(Account account);
        Task<bool> EmailExistsAsync(string email);
        Task<Account?> GetByEmailAsync(string email);
        Task<bool> EmployeeExistsAsync(int employeeId);

    }
}