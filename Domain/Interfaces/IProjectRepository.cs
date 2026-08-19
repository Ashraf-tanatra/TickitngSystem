using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        IEnumerable<Project> GetAll();

        Project? GetById(int id);

        void Add(Project project);

        void Update(Project project);

        void Delete(Project project);

        bool EmployeeExists(int employeeId); // ?

        //bool IsManager(int employeeId); // ?
        IEnumerable<Employee> GetEmployees(int projectId);
        IEnumerable<Ticket> GetTickets(int projectId);
    }
}