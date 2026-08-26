using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task<Employee?> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<bool> ExistsByPhoneAsync(string phone);
        Task<IEnumerable<Project>> GetProjectsAsync(int employeeId);
        Task<IEnumerable<Ticket>> GetEmployeeTickets(int employeeId);
        Task<bool> ExistsByPhoneExceptAsync(string phone, int employeeId);
        Task<IEnumerable<Project>> GetActiveProjectsAsync(int employeeId);
    }
}