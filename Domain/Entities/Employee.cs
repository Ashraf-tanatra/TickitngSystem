using Domain.Enum;

namespace Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public Gender Gender { get; set; }
        public Account? Account { get; set; }
        public DateOnly? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
    }
}