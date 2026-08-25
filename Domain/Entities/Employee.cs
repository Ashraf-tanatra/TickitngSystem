using Domain.Enum;

namespace Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Phone { get; set; }
        public Gender Gender { get; set; }
        public Account? Account { get; set; }
        public DateOnly? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }= new List<ProjectEmployee>();


    }
}