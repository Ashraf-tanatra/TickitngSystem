using ApplicationServices.DTOs;

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
    }
}