using ApplicationServices.DTOs.Project;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface IProjectManager
    {
        Task<bool> DeleteAsync(int id, int empId);
        Task<ProjectResponse?> GetByIdAsync(int id);
        Task<int> GetProjectCountAsync(int employeeId);
        Task<int> CreateAsync(CreateProjectRequest request);
        Task<bool> ProjectAddEmployeeAsync(ProjectEmployeeRequest request);
        Task<bool> SetProjectStatusAsync(int projectId, ProjectStatus status);
        Task<bool> UpdateAsync(int projectId, int empId, UpdateProjectRequest request);
        Task<IEnumerable<EmployeeResponse>>? GetEmployeesWorkOnProjectAsync(int projectId);
        Task<IEnumerable<ProjectResponse>>? GetAllProjectWorkedByEmployeeAsync(int employeeId);
        Task<IEnumerable<string[]>>? GetAllProjectWorkedByEmployeeTopThreeAsync(int employeeId);
        Task<IEnumerable<ProjectResponse>>? GetAllProjectWorkedByEmployeeWithFilterAsync(int employeeId, ProjectStatus FilterByStatus);
    }
}