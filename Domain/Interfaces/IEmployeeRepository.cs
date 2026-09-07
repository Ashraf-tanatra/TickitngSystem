using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(int id);

        Task AddAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task<bool> ExistsByPhoneAsync(string phone);

        Task<bool> ExistsByPhoneExceptAsync(
            string phone,
            int employeeId);

        Task<IEnumerable<Project>> GetProjectsAsync(
            int employeeId);

        Task<IEnumerable<Project>> GetActiveProjectsAsync(int employeeId);
        Task<IEnumerable<Ticket>> GetEmployeeTicketsAsync(int employeeId);
    }
}
