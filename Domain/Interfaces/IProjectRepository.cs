using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Project project);
        Task<int> GetProjectCountAsync(int employeeId);
        Task<bool> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int projectId, int employeeId);
        Task<IEnumerable<Employee>?> GetEmployeesAsync(int employeeId);
        Task<bool> AddEmployeeToProjectAsync(ProjectEmployee projectEmployee);
        Task<bool> SetProjectStatusAsync(int projectId, ProjectStatus status);
        Task<IEnumerable<Project>?> GetAllProjectWorkedByEmployeeAsync(int employeeId);
        Task<IEnumerable<String[]>?> GetAllProjectWorkedByEmployeeTopThreeAsync(int employeeId);
        Task<IEnumerable<Project>?> GetAllProjectWorkedByEmployeeWithFilterAsync(int employeeId, ProjectStatus FilterByStatus);

        Task<bool> TicketExistsAsync(int ticketId);
        Task<bool> ProjectExistsAsync(int projectId);
        Task<bool> EmployeeExistsAsync(int employeeId);
        Task<bool> IsManagerAsync(int projectId, int employeeId);

    }
}