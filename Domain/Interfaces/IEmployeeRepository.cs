using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(Guid id);

        Task AddAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task<bool> ExistsByPhoneAsync(string phone);

        Task<bool> ExistsByPhoneExceptAsync(
            string phone,
            Guid employeeId);

        Task<IEnumerable<Project>> GetProjectsAsync(
            Guid employeeId);

        Task<IEnumerable<Project>> GetActiveProjectsAsync(Guid employeeId);
        Task<IEnumerable<Ticket>> GetEmployeeTicketsAsync(Guid employeeId);
    }
}
