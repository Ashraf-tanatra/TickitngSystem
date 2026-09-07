using Domain.Entities;

namespace Domain.Interfaces
{
    
    public interface IProjectEmployeeRepository
    {
        Task<IEnumerable<ProjectEmployee>> GetAllAsync();
        Task<IEnumerable<ProjectEmployee>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<ProjectEmployee>> GetByEmployeeIdAsync(int employeeId);

        Task<ProjectEmployee?> GetAsync(int projectId, int employeeId);

        Task<bool> ExistsAsync(int projectId, int employeeId);

        Task AddAsync(ProjectEmployee projectEmployee);

        Task DeleteAsync(ProjectEmployee projectEmployee);
    }
}
