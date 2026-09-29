using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface IProjectManager
    {
        Task<bool> DeleteAsync(Guid id, Guid empId);
        Task<ProjectResponse?> GetByIdAsync(Guid id);
        Task<int> GetProjectCountAsync(Guid employeeId);
        Task<Guid> CreateAsync(CreateProjectRequest request, Guid managerId);
        Task<bool> ProjectAddEmployeeAsync(ProjectEmployeeRequest request, Guid actionByEmployeeId);
        Task<bool> RemoveEmployeeFromProjectAsync(RemoveProjectEmployeeRequest request, Guid actionByEmployeeId);
        Task<bool> SetProjectStatusAsync(Guid projectId, ProjectStatus status, Guid actionByEmployeeId);
        Task<bool> UpdateAsync(Guid projectId, Guid empId, UpdateProjectRequest request);
        Task<IEnumerable<EmployeeResponse>>? GetEmployeesWorkOnProjectAsync(Guid projectId);
        Task<IEnumerable<ProjectResponse>>? GetAllProjectWorkedByEmployeeAsync(Guid employeeId);
        Task<IEnumerable<ProjectResponse>> GetDashboardProjectsAsync(Guid employeeId);
        Task<IEnumerable<ProjectResponse>> GetRecentActiveProjectsAsync(Guid employeeId);
        Task<IEnumerable<RecentActivityResponse>> GetRecentActivityAsync(Guid employeeId);
        Task<IEnumerable<ProjectResponse>>? GetAllProjectWorkedByEmployeeWithFilterAsync(Guid employeeId, ProjectStatus FilterByStatus);
    }
}
