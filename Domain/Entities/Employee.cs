using Domain.Enum;

namespace Domain.Entities
{
    public class Employee
    {
        public int Id { get; }

        public required string FName { get; set; }

        public required string LName { get; set; }

        public string Phone { get; set; }

        public Gender Gender { get; set; }

        public Account Account { get; set; }

        public EmployeeRole Role { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
            = new List<ProjectEmployee>();

        public ICollection<Ticket> Tickets { get; set; }
            = new List<Ticket>();


    }
}