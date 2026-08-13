using ApplicationServices.DTOs;

namespace Domain.Interfaces
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
    }
}