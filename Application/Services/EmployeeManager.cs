using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Domain.EntityManager
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountRepository _accountRepository;

        public EmployeeManager(IEmployeeRepository employeeRepository, IAccountRepository accountRepository)
        {
            _employeeRepository = employeeRepository;
            _accountRepository = accountRepository;

        }


        // CREATE
        //public EmployeeResponse Create(CreateEmployeeRequest request)
        //{
        //    if (request == null)
        //        throw new ArgumentNullException(nameof(request));

        //    if (string.IsNullOrWhiteSpace(request.FName))
        //        throw new ArgumentException("First name is required.");

        //    if (string.IsNullOrWhiteSpace(request.LName))
        //        throw new ArgumentException("Last name is required.");

        //    if (string.IsNullOrWhiteSpace(request.Email))
        //        throw new ArgumentException("Email is required.");

        //    if (string.IsNullOrWhiteSpace(request.Phone))
        //        throw new ArgumentException("Phone is required.");

        //    if (string.IsNullOrWhiteSpace(request.Password))
        //        throw new ArgumentException("Password is required.");

        //    if (request.Password != request.ConfirmPassword)
        //        throw new ArgumentException(
        //            "Password and confirm password do not match.");

        //    if (!request.AcceptTerms)
        //        throw new ArgumentException(
        //            "You must accept the Terms of Service and Privacy Policy.");

        //    // Check Email
        //    if (_accountRepository.EmailExists(request.Email))
        //        throw new InvalidOperationException(
        //            "An account with this email already exists.");

        //    // Check Phone
        //    if (_employeeRepository.ExistsByPhone(request.Phone))
        //        throw new InvalidOperationException(
        //            "An employee with this phone already exists.");

        //    // Create Employee and Account
        //    var employee = new Employee
        //    {
        //        FName = request.FName,
        //        LName = request.LName,
        //        Phone = request.Phone,
        //        Gender = (Enum.Gender)request.Gender
        //    };

        //    var account = new Account
        //    {
        //        Email = request.Email,
        //        PasswordHash = request.Password,
        //        Employee = employee
        //    };

        //    employee.Account = account;

        //    _employeeRepository.Add(employee);

        //    return MapToResponse(employee);
        //}


        // GET ALL
        public IEnumerable<EmployeeResponse> GetAll()
        {
            var employees = _employeeRepository.GetAll();

            return employees.Select(MapToResponse);
        }


        // GET BY ID
        public EmployeeResponse? GetById(int id)
        {
            var employee = _employeeRepository.GetById(id);

            if (employee == null)
                return null;

            return MapToResponse(employee);
        }

        // Update
        public EmployeeResponse? Update(int id, UpdateEmployeeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var employee = _employeeRepository.GetById(id);

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
            if (_employeeRepository.ExistsByPhoneExcept(
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

            _employeeRepository.Update(employee);

            return MapToResponse(employee);
        }


        // DELETE
        public bool Delete(int id)
        {
            var employee = _employeeRepository.GetById(id);

            if (employee == null)
                return false;

            if (employee.IsDeleted)
                throw new InvalidOperationException(
                    "Employee is already deleted.");

            employee.IsDeleted = true;
            employee.DeletedAt = DateTime.UtcNow;

            var account = employee.Account;
            _accountRepository.Delete(account);

            _employeeRepository.Update(employee);

            return true;
        }


        // GET EMPLOYEE PROJECTS

        public IEnumerable<EmployeeProjectResponse> GetProjects(int employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);

            if (employee == null)
                throw new KeyNotFoundException(
                    "Employee not found.");

            var projects = _employeeRepository.GetProjects(employeeId);

            return projects.Select(project =>
            {
                var projectEmployee = project.ProjectEmployees
                    .First(pe => pe.EmployeeId == employeeId);

                return new EmployeeProjectResponse
                {
                    Id = project.Id,

                    ProjectName = project.ProjectName,

                    ProjectDescription = project.ProjectDescription,

                    Role = projectEmployee.Role.ToString(),

                    EmployeeCount = project.ProjectEmployees.Count,

                    TicketCount = project.ProjectTickets.Count
                };
            });
        }

        //ReActive Employee
        public bool Reactivate(int id,ReactivateAccountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(
                    "Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException(
                    "Password is required.");

            if (request.Password != request.ConfirmPassword)
                throw new ArgumentException(
                    "Password and confirm password do not match.");

            // =========================
            // Get Employee
            // =========================

            var employee = _employeeRepository.GetById(id);

            if (employee == null)
                return false;

            if (!employee.IsDeleted)
                throw new InvalidOperationException(
                    "Employee is already active.");

            // =========================
            // Check Deleted Date
            // =========================

            if (employee.DeletedAt == null)
                throw new InvalidOperationException(
                    "Deleted date is not available.");

            if (employee.DeletedAt.Value.AddDays(10) < DateTime.UtcNow)
                throw new InvalidOperationException(
                    "Employee cannot be reactivated after 10 days.");

            // =========================
            // Check Email
            // =========================

            if (_accountRepository.EmailExists(request.Email))
                throw new InvalidOperationException(
                    "An account with this email already exists.");

            // =========================
            // Reactivate Employee
            // =========================

            employee.IsDeleted = false;
            employee.DeletedAt = null;

            _employeeRepository.Update(employee);

            // =========================
            // Create New Account
            // =========================

            var account = new Account
            {
                Email = request.Email,
                PasswordHash = request.Password,
                EmployeeId = employee.Id
            };

            _accountRepository.Add(account);

            return true;
        }

        // ADD
        public void Add(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _employeeRepository.Add(employee);
        }

        // VALIDATE PHONE NUMBER
        public bool ValidPhoneNumberFormat(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone)
                   && phone.Length == 10
                   && phone.All(char.IsDigit);
        }

        // CHECK PHONE
        public bool ExistsByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return _employeeRepository.ExistsByPhone(phone);
        }

        // MAP EMPLOYEE TO RESPONSE
        private static EmployeeResponse MapToResponse(Employee employee)
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