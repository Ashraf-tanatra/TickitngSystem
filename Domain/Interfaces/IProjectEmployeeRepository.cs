using Domain.Entities;

namespace Domain.Interfaces
{
    
    public interface IProjectEmployeeRepository
    {
        Task<IEnumerable<ProjectEmployee>> GetAllAsync();
        Task<IEnumerable<ProjectEmployee>> GetByProjectIdAsync(Guid projectId);
        Task<IEnumerable<ProjectEmployee>> GetByEmployeeIdAsync(Guid employeeId);

        Task<ProjectEmployee?> GetAsync(Guid projectId, Guid employeeId);

        Task<bool> ExistsAsync(Guid projectId, Guid employeeId);

        Task AddAsync(ProjectEmployee projectEmployee);

        Task DeleteAsync(ProjectEmployee projectEmployee);
    }
}
