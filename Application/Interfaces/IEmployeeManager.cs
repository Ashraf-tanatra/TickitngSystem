using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IEmployeeManager
    {
        Task<bool> DeleteAsync(int id);
        Task AddAsync(Employee employee);
        Task<bool> ReactivateAsync(int id);
        bool ValidPhoneNumberFormat(string phone);
        Task<bool> ExistsByPhoneAsync(string phone);
        Task<EmployeeResponse?> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeResponse>> GetAllAsync();
        Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request);
        Task<IEnumerable<EmployeeProjectResponse>> GetProjectsAsync(int employeeId);
        //Task<IEnumerable<ProjectResponse>>GetActiveProjectsAsync(int employeeId);
    }
}