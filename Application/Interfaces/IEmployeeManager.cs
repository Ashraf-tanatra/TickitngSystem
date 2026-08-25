using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IEmployeeManager
    {
        Task<EmployeeResponse?> GetByIdAsync(int id);

        Task<IEnumerable<EmployeeResponse>> GetAllAsync();

        Task<EmployeeResponse?> UpdateAsync(
            int id,
            UpdateEmployeeRequest request);

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<EmployeeProjectResponse>> GetProjectsAsync(
            int employeeId);

        Task AddAsync(Employee employee);

        bool ValidPhoneNumberFormat(string phone);

        Task<bool> ExistsByPhoneAsync(string phone);
    }
}