using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IEmployeeManager
    {
        EmployeeResponse Create(CreateEmployeeRequest request);

        EmployeeResponse? GetById(int id);

        IEnumerable<EmployeeResponse> GetAll();

        EmployeeResponse? Update(
            int id,
            UpdateEmployeeRequest request);

        bool Delete(int id);
        IEnumerable<ProjectResponse> GetProjects(int employeeId);



    }
}