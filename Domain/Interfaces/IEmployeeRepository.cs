using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAll();

        Employee? GetById(int id);

        void Add(Employee employee);

        void Update(Employee employee);

        void Delete(Employee employee);

        bool ExistsByEmail(string email); // ?

        bool ExistsByPhone(string phone);
        bool ExistsByPhoneExcept(string phone, int employeeId);
        IEnumerable<Project> GetProjects(int employeeId);

    }
}