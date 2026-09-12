using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountRepository _accountRepository;

        public EmployeeManager(IEmployeeRepository employeeRepository,IAccountRepository accountRepository)
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
                    ErrorShared.Employee.CannotUpdateDeletedEmployee);

            if (string.IsNullOrWhiteSpace(request.FName))
                throw new ArgumentException(
                    ErrorShared.Employee.FirstNameRequired);

            if (string.IsNullOrWhiteSpace(request.LName))
                throw new ArgumentException(
                    ErrorShared.Employee.LastNameRequired);

            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new ArgumentException(
                    ErrorShared.Employee.PhoneRequired);

            // =====================================================
            // VALIDATE PHONE
            // =====================================================

            if (!ValidPhoneNumberFormat(request.Phone))
                throw new ArgumentException(
                    ErrorShared.Employee.InvalidPhoneNumber);

            // =====================================================
            // CHECK DUPLICATE PHONE
            // =====================================================

            if (await _employeeRepository
                .ExistsByPhoneExceptAsync(
                    request.Phone,
                    id))
            {
                throw new InvalidOperationException(
                    ErrorShared.Employee.PhoneAlreadyExists);
            }

            // =====================================================
            // UPDATE EMPLOYEE
            // =====================================================

            employee.UpdateDetails(
                request.FName,
                request.LName,
                request.Phone,
                request.Gender);

            await _employeeRepository.UpdateAsync(employee);

            return MapToResponse(employee);
        }

        public async Task<EmployeeResponse?> UpdateProfileImageAsync(
            int id,
            string profileImageUrl)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return null;

            if (employee.IsDeleted)
                throw new InvalidOperationException(
                    ErrorShared.Employee.CannotUpdateDeletedEmployee);

            employee.ChangeProfileImage(profileImageUrl);

            await _employeeRepository.UpdateAsync(employee);

            return MapToResponse(employee);
        }

        //// =========================================================
        //// ACTIVE PROJECTS FOR EMPLOYEE
        //// =========================================================
        //public async Task<IEnumerable<ProjectResponse>>GetActiveProjectsAsync(int employeeId)
        //{
        //    var employee =
        //        await _employeeRepository.GetByIdAsync(employeeId);

        //    if (employee == null)
        //        throw new KeyNotFoundException(
        //            "Employee not found.");

        //    if (employee.IsDeleted)
        //        throw new InvalidOperationException(
        //            "Employee is deleted.");

        //    var projects =
        //        await _employeeRepository
        //            .GetActiveProjectsAsync(employeeId);

        //    return projects.Select(MapToResponse);
        //}

        // =========================================================
        // SOFT DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(int id)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return false;

            if (employee.IsDeleted)
                throw new InvalidOperationException(
                    ErrorShared.Employee.EmployeeAlreadyDeleted);

            // =====================================================
            // CHECK PROJECT MANAGER
            // =====================================================

            var managedProjects = await _employeeRepository.GetActiveProjectsAsync(id);

            if (managedProjects.Any())
            {
                throw new InvalidOperationException(
                    ErrorShared.Employee.CannotDeleteProjectManager);
            }

            var employeeTickets = await _employeeRepository.GetEmployeeTicketsAsync(id);
            var hasActiveTickets = employeeTickets.Any(ticket =>
                ticket.TicketStatus != TicketStatus.Done &&
                ticket.TicketStatus != TicketStatus.Completed &&
                ticket.TicketStatus != TicketStatus.Cancelled);

            if (hasActiveTickets)
                throw new InvalidOperationException(
                    ErrorShared.Employee.EmployeeHasActiveTickets);


            // =====================================================
            // SOFT DELETE EMPLOYEE
            // =====================================================

            employee.Deactivate();

            // =====================================================
            // SOFT DELETE ACCOUNT
            // =====================================================

            var account = employee.Account;

            if (account != null && !account.IsDeleted)
            {
                await _accountRepository
                    .SoftDeleteAsync(account);
            }

            // =====================================================
            // SAVE EMPLOYEE
            // =====================================================

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
                    ErrorShared.Employee.EmployeeNotFound);

            if (employee.IsDeleted)
                throw new InvalidOperationException(
                    ErrorShared.Employee.CannotUpdateDeletedEmployee);

            var projects =
                await _employeeRepository
                    .GetProjectsAsync(employeeId);

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
                        Role = projectEmployee.Role ?? ErrorShared.Employee.NoRole,
                        EmployeeCount =
                            project.ProjectEmployees.Count(projectEmployee =>
                                !projectEmployee.Employee.IsDeleted),
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
                ProfileImageUrl = employee.ProfileImageUrl,
                IsDeleted = employee.IsDeleted
            };
        }


        // =========================================================
        // REACTIVE EMPLOYEE
        // =========================================================

        public async Task<bool> ReactivateAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return false;

            if (!employee.IsDeleted)
                throw new InvalidOperationException(
                    ErrorShared.Employee.EmployeeAlreadyActive);

            if (!employee.DeletedAt.HasValue)
                throw new InvalidOperationException(
                    ErrorShared.Employee.EmployeeDeletionDateMissing);

            if (employee.DeletedAt.Value.AddDays(ErrorShared.Employee.ReactivationPeriodDays) <
                DateOnly.FromDateTime(DateTime.UtcNow))
                throw new InvalidOperationException(
                    ErrorShared.Employee.EmployeeReactivationPeriodExpired);

            employee.Reactivate();

            var account = employee.Account;

            if (account != null && account.IsDeleted)
            {
                await _accountRepository.ReactivateAsync(account);
            }

            await _employeeRepository.UpdateAsync(employee);

            return true;
        }
    }
}
