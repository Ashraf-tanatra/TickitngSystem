using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(Project project);
        Task<int> GetProjectCountAsync(Guid employeeId);
        Task<bool> UpdateAsync(Project project);
        Task<bool> DeleteAsync(Guid projectId, Guid employeeId);
        Task<IEnumerable<Employee>?> GetEmployeesAsync(Guid employeeId);
        Task<bool> AddEmployeeToProjectAsync(ProjectEmployee projectEmployee);
        Task<bool> RemoveEmployeeFromProjectAsync(Guid projectId, Guid employeeId);
        Task<bool> EmployeeHasActiveTicketsInProjectAsync(Guid projectId, Guid employeeId);
        Task<bool> SetProjectStatusAsync(Guid projectId, ProjectStatus status);
        Task<IEnumerable<Project>?> GetAllProjectWorkedByEmployeeAsync(Guid employeeId);
        Task<IEnumerable<Project>?> GetDashboardProjectsAsync(Guid employeeId);
        Task<IEnumerable<Project>> GetRecentActiveProjectsAsync(Guid employeeId);
        Task<IEnumerable<Project>?> GetAllProjectWorkedByEmployeeWithFilterAsync(Guid employeeId, ProjectStatus FilterByStatus);

        Task<bool> TicketExistsAsync(Guid ticketId);
        Task<bool> ProjectExistsAsync(Guid projectId);
        Task<bool> EmployeeExistsAsync(Guid employeeId);
        Task<bool> IsManagerAsync(Guid projectId, Guid employeeId);

    }
}
