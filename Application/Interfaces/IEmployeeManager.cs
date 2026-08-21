using ApplicationServices.DTOs;
using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IEmployeeManager
    {
        //EmployeeResponse Create(CreateEmployeeRequest request);

        EmployeeResponse? GetById(int id);

        IEnumerable<EmployeeResponse> GetAll();

        EmployeeResponse? Update(
            int id,
            UpdateEmployeeRequest request);

        bool Delete(int id);

        IEnumerable<EmployeeProjectResponse> GetProjects(int employeeId);

        void Add(Employee employee);

        bool ValidPhoneNumberFormat(string phone);

        bool ExistsByPhone(string phone);
        bool Reactivate(int id,ReactivateAccountRequest request);
    }
}