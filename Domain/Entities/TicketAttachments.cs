namespace Domain.Entities
{
    public class TicketAttachments
    {
        public int Id { get; set; }
        public string URL { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}