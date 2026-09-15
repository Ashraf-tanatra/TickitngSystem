using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IEmployeeManager
    {
        Task<EmployeeResponse?> GetByIdAsync(Guid id);

        Task<IEnumerable<EmployeeResponse>> GetAllAsync();

        Task<EmployeeResponse?> UpdateAsync(
            Guid id,
            UpdateEmployeeRequest request);

        Task<EmployeeResponse?> UpdateProfileImageAsync(
            Guid id,
            string profileImageUrl);

        Task<bool> DeleteAsync(Guid id);

        Task<IEnumerable<EmployeeProjectResponse>> GetProjectsAsync(
            Guid employeeId);

        Task AddAsync(Employee employee);

        bool ValidPhoneNumberFormat(string phone);

        Task<bool> ReactivateAsync(Guid id);
        Task<bool> ExistsByPhoneAsync(string phone);
        //Task<IEnumerable<ProjectResponse>>GetActiveProjectsAsync(Guid employeeId);
    }
}
