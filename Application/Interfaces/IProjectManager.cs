using ApplicationServices.DTOs.Project;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface IProjectManager
    {
        IEnumerable<ProjectResponse>? GetAllProjectWorkedByEmployee(int employeeId);
        IEnumerable<string[]>? GetAllProjectWorkedByEmployeeTopThree(int employeeId);
        IEnumerable<EmployeeResponse>? GetEmployeesWorkOnProject(int projectId);
        int GetProjectCount(int employeeId);

        int Create(CreateProjectRequest request);
        ProjectResponse? GetById(int id);
        void Update(int projectId, int empId, UpdateProjectRequest request);
        void Delete(int id, int empId);

        void ProjectAddEmployee(ProjectEmployeeRequest request);

        void SetProjectStatus(int projectId, ProjectStatus status);

        bool ProjectExits(int projectId);


        IEnumerable<ProjectResponse>? GetAllProjectWorkedByEmployeeWithFilter(int employeeId, ProjectStatus FilterByStatus);
    }
}