using Domain.Enum;

namespace Domain.Entities
{
    public class Employee : BaseEntity
    {
        private readonly List<Ticket> _tickets = new();
        private readonly List<Project> _managedProjects = new();
        private readonly List<ProjectEmployee> _projectEmployees = new();

        private Employee()
        {
        }

        public string FName { get; private set; } = string.Empty;
        public string LName { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public Gender Gender { get; private set; }
        public Account? Account { get; private set; }
        public DateOnly? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public IReadOnlyCollection<Ticket> Tickets => _tickets;
        public IReadOnlyCollection<Project> ManagedProjects => _managedProjects;
        public IReadOnlyCollection<ProjectEmployee> ProjectEmployees => _projectEmployees;

        public static Employee Create(
            string fName,
            string lName,
            string phone,
            Gender gender)
        {
            var employee = new Employee();
            employee.UpdateDetails(fName, lName, phone, gender);
            return employee;
        }

        public void UpdateDetails(
            string fName,
            string lName,
            string phone,
            Gender gender)
        {
            if (string.IsNullOrWhiteSpace(fName))
                throw new ArgumentException(ErrorShared.Employee.FirstNameRequired);

            if (string.IsNullOrWhiteSpace(lName))
                throw new ArgumentException(ErrorShared.Employee.LastNameRequired);

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException(ErrorShared.Employee.PhoneRequired);

            if (IsDeleted)
                throw new InvalidOperationException(ErrorShared.Employee.CannotUpdateDeletedEmployee);

            FName = fName.Trim();
            LName = lName.Trim();
            Phone = phone.Trim();
            Gender = gender;
            Touch();
        }

        public void AttachAccount(Account account)
        {
            ArgumentNullException.ThrowIfNull(account);
            Account = account;
            Touch();
        }

        public void Deactivate()
        {
            if (IsDeleted)
                throw new InvalidOperationException(ErrorShared.Employee.EmployeeAlreadyDeleted);

            IsDeleted = true;
            DeletedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            Touch();
        }

        public void Reactivate()
        {
            if (!IsDeleted)
                throw new InvalidOperationException(ErrorShared.Employee.EmployeeAlreadyActive);

            IsDeleted = false;
            DeletedAt = null;
            Touch();
        }
    }
}
