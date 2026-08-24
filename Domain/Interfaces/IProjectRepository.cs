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
        void Delete(Project project);

        void AddEmployeeToProject(ProjectEmployee projectEmployee);

        void SetProjectAsActive(int projectId);
        void SetProjectAsCancelled(int projectId);
        void SetProjectAsCompleted(int projectId);
        void SetProjectAsOnHold(int projectId);

        bool EmployeeExists(int employeeId); // ?

        IEnumerable<Project>? GetAllProjectWorkedByEmployeeWithFilter(int employeeId, ProjectStatus FilterByStatus);


        //bool IsManager(int employeeId); // ?

        //IEnumerable<Ticket> GetTickets(int projectId);

    }
}