namespace Domain.Entities
{
    public class TicketAttachments
    {
        public int Id { get; set; }
        public int TicketId { get; set; } // Foreign key
        public string URL { get; set; } = null!;
        public Ticket Ticket { get; set; } = null!; // Navigation property
    }
}