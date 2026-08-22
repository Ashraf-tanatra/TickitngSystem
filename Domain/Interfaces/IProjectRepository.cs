using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        //IEnumerable<Project> GetAll();
        IEnumerable<Project>? GetAllProjectWorkedByEmployee(int employeeId);
        IEnumerable<String[]>? GetAllProjectWorkedByEmployeeTopThree(int employeeId);
        IEnumerable<Employee>? GetEmployees(int projectId);
        int GetProjectCount(int employeeId);
        Project? GetById(int id);

        void Create(Project project);
        void Update(Project project);
        void Delete(int projectId, int employeeId);

        void AddEmployeeToProject(ProjectEmployee projectEmployee);

        void SetProjectStatus(int projectId, ProjectStatus status);

        bool EmployeeExists(int employeeId);
        bool ProjectExits(int projectId);
        bool TicketExists(int ticketId);
        bool IsManager(int projectId, int employeeId);

        IEnumerable<Project>? GetAllProjectWorkedByEmployeeWithFilter(int employeeId, ProjectStatus FilterByStatus);

    }
}