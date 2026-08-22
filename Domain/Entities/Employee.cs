using Domain.Enum;

namespace Domain.Entities
{
    public class Employee
    {
        public int Id { get; }

        public required string FName { get; set; } = null!;

        public required string LName { get; set; } = null!;

        public string Phone { get; set; } = null!;  // unique

        public Gender Gender { get; set; }

        public Account Account { get; set; }
        public DateTime? DeletedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public const int DeleteTimePeriod = 30;

        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
            = new List<ProjectEmployee>();

        public ICollection<Ticket> Tickets { get; set; }
            = new List<Ticket>();
        public ICollection<TicketHistory> TicketHistories { get; set; }
            = new List<TicketHistory>();

    }
}