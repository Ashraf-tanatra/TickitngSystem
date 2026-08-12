
using ApplicationServices.DTOs;
namespace Domain.Interfaces
{
    public interface IEmployeeManager
    {
        Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);

        Task<EmployeeResponse?> GetByIdAsync(int id);

        Task<IEnumerable<EmployeeResponse>> GetAllAsync();

        Task<EmployeeResponse?> UpdateAsync(
            int id,
            UpdateEmployeeRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
