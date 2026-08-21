using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        //IEnumerable<Project> GetAll();
        IEnumerable<Project>? GetAllProjectWorkedByEmployee(int employeeId);
        IEnumerable<String[]>? GetAllProjectWorkedByEmployeeTopThree(int employeeId);
        int GetProjectCount(int employeeId);
        Project? GetById(int id);

        void Create(Project project);
        void Update(Project project);
        void Delete(Project project);

        void SetProjectAsActive(int projectId);
        void SetProjectAsCancelled(int projectId);
        void SetProjectAsCompleted(int projectId);
        void SetProjectAsOnHold(int projectId);

        bool EmployeeExists(int employeeId); // ?


        //bool IsManager(int employeeId); // ?
        //IEnumerable<Employee> GetEmployees(int projectId);
        //IEnumerable<Ticket> GetTickets(int projectId);

    }
}