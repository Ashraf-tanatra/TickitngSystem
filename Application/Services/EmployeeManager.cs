using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeRepository _EmployeeRepository;

        public EmployeeManager(IEmployeeRepository repository)
        {
            _EmployeeRepository = repository;
        }

        public EmployeeResponse Create(CreateEmployeeRequest request)
        {
            // 1. Validate request
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.FName))
                throw new ArgumentException("First name is required.");

            if (string.IsNullOrWhiteSpace(request.LName))
                throw new ArgumentException("Last name is required.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new ArgumentException("Phone is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");


            // 2. Check existing employees
            if (_EmployeeRepository.ExistsByEmail(request.Email))
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            if (_EmployeeRepository.ExistsByPhone(request.Phone))
            {
                throw new InvalidOperationException(
                    "An employee with this phone already exists.");
            }


            // 3. Create Account
            var account = new Account
            {
                Email = request.Email,
                PasswordHash = request.Password
            };


            // 4. Create Employee
            var employee = new Employee
            {
                FName = request.FName,
                LName = request.LName,
                Phone = request.Phone,
                Gender = request.Gender
            };


            // 5. Save Employee
            _EmployeeRepository.Add(employee);


            // 6. Return Response
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

        public bool Delete(int id)
        {
            // 1. Get employee
            var employee = _EmployeeRepository.GetById(id);

            // 2. Employee doesn't exist
            if (employee == null)
                return false;

            // 3. Delete employee
            _EmployeeRepository.Delete(employee);

            // 4. Successfully deleted
            return true;
        }

        public IEnumerable<EmployeeResponse> GetAll()
        {
            var employees = _EmployeeRepository.GetAll();

            return employees.Select(employee => new EmployeeResponse
            {
                Id = employee.Id,
                FName = employee.FName,
                LName = employee.LName,
                Phone = employee.Phone,
                Gender = employee.Gender,
                IsDeleted = employee.IsDeleted
            });
        }

        public EmployeeResponse? GetById(int id)
        {
            var employee = _EmployeeRepository.GetById(id);

            if (employee == null)
                return null;

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

        public EmployeeResponse? Update(int id, UpdateEmployeeRequest request)
        {
            // 1. Validate request
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // 2. Get employee
            var employee = _EmployeeRepository.GetById(id);

            if (employee == null)
                return null;

            // 3. Validate fields
            if (string.IsNullOrWhiteSpace(request.FName))
                throw new ArgumentException("First name is required.");

            if (string.IsNullOrWhiteSpace(request.LName))
                throw new ArgumentException("Last name is required.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new ArgumentException("Phone is required.");


            // 4. Check if phone belongs to another employee
            if (_EmployeeRepository.ExistsByPhoneExcept(request.Phone, id))
            {
                throw new InvalidOperationException("This phone number is already in use.");
            }


            // 5. Update employee
            employee.FName = request.FName;
            employee.LName = request.LName;
            employee.Phone = request.Phone;
            employee.Gender = request.Gender;


            // 6. Save changes
            _EmployeeRepository.Update(employee);


            // 7. Return response
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

        public IEnumerable<ProjectResponse> GetProjects(int employeeId)
        {
            var projects = _EmployeeRepository.GetProjects(employeeId);

            return projects.Select(project => new ProjectResponse
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectManagerId = project.ProjectManagerId,

                ProjectManagerName = project.ProjectManager == null
                    ? null
                    : $"{project.ProjectManager.FName} {project.ProjectManager.LName}",

                EmployeeCount = project.ProjectEmployees.Count,
                TicketCount = project.ProjectTickets.Count
            });
        }


    }
}