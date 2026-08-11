namespace Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }

        public Account? Account { get; set; }

        public string Phone { get; set; }

        public string Gender { get; set; }

        public bool IsAvilable { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public Ticket? Ticket { get; set; }







    }
}
