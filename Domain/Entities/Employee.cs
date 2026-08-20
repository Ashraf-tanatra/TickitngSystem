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

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public Account? Account { get; set; }

        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
            = new List<ProjectEmployee>();

        public ICollection<Ticket> Tickets { get; set; }
            = new List<Ticket>();

        public ICollection<TicketHistory> TicketHistories { get; set; }
            = new List<TicketHistory>();
    }
}