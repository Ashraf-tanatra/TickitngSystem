using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountRepository _accountRepository;

        public EmployeeManager(
            IEmployeeRepository employeeRepository,
            IAccountRepository accountRepository)
        {
            _employeeRepository = employeeRepository;
            _accountRepository = accountRepository;
        }

        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<IEnumerable<EmployeeResponse>> GetAllAsync()
        {
            var employees =
                await _employeeRepository.GetAllAsync();

            return employees.Select(MapToResponse);
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<EmployeeResponse?> GetByIdAsync(int id)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return null;

            return MapToResponse(employee);
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task<EmployeeResponse?> UpdateAsync(
            int id,
            UpdateEmployeeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return null;

            if (employee.IsDeleted)
                throw new InvalidOperationException(
                    "Cannot update a deleted employee.");

            if (string.IsNullOrWhiteSpace(request.FName))
                throw new ArgumentException(
                    "First name is required.");

            if (string.IsNullOrWhiteSpace(request.LName))
                throw new ArgumentException(
                    "Last name is required.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new ArgumentException(
                    "Phone is required.");

            // Validate phone format
            if (!ValidPhoneNumberFormat(request.Phone))
                throw new ArgumentException(
                    "Phone number must contain exactly 10 digits.");

            // Check duplicate phone
            if (await _employeeRepository.ExistsByPhoneExceptAsync(
                    request.Phone,
                    id))
            {
                throw new InvalidOperationException(
                    "This phone number is already in use.");
            }

            employee.FName = request.FName;
            employee.LName = request.LName;
            employee.Phone = request.Phone;
            employee.Gender = request.Gender;

            await _employeeRepository.UpdateAsync(employee);

            return MapToResponse(employee);
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(int id)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return false;

            if (employee.IsDeleted)
                throw new InvalidOperationException(
                    "Employee is already deleted.");

            employee.IsDeleted = true;
            employee.DeletedAt = DateTime.UtcNow;

            var account = employee.Account;

            if (account != null)
            {
                await _accountRepository.DeleteAsync(account);
            }

            await _employeeRepository.UpdateAsync(employee);

            return true;
        }

        // =========================================================
        // GET EMPLOYEE PROJECTS
        // =========================================================

        public async Task<IEnumerable<EmployeeProjectResponse>>
            GetProjectsAsync(int employeeId)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                throw new KeyNotFoundException(
                    "Employee not found.");

            var projects =
                await _employeeRepository.GetProjectsAsync(employeeId);

            return projects
                .Where(project =>
                    project.ProjectEmployees
                        .Any(pe => pe.EmployeeId == employeeId))
                .Select(project =>
                {
                    var projectEmployee =
                        project.ProjectEmployees
                            .First(pe => pe.EmployeeId == employeeId);

                    return new EmployeeProjectResponse
                    {
                        Id = project.Id,
                        ProjectName = project.ProjectName,
                        ProjectDescription =
                            project.ProjectDescription,
                        Role = projectEmployee.Role.ToString(),
                        EmployeeCount =
                            project.ProjectEmployees.Count,
                        TicketCount =
                            project.ProjectTickets.Count
                    };
                });
        }

        // =========================================================
        // ADD
        // =========================================================

        public async Task AddAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            await _employeeRepository.AddAsync(employee);
        }

        // =========================================================
        // VALIDATE PHONE NUMBER
        // =========================================================

        public bool ValidPhoneNumberFormat(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone)
                   && phone.Length == 10
                   && phone.All(char.IsDigit);
        }

        // =========================================================
        // CHECK PHONE
        // =========================================================

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return await _employeeRepository
                .ExistsByPhoneAsync(phone);
        }

        // =========================================================
        // MAP EMPLOYEE TO RESPONSE
        // =========================================================

        private static EmployeeResponse MapToResponse(
            Employee employee)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                FName = employee.FName,
                LName = employee.LName,
                Phone = employee.Phone,
                Gender = employee.Gender,
                IsDeleted = employee.IsDeleted
            };
        }
    }
}