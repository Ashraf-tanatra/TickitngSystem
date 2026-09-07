namespace Domain.Entities
{
    public class TicketAttachments : BaseEntity
    {
        private TicketAttachments()
        {
        }

        public string URL { get; private set; } = string.Empty;
        public int TicketId { get; private set; }
        public Ticket Ticket { get; private set; } = null!;

        public static TicketAttachments Create(int ticketId, string url)
        {
            if (ticketId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.TicketNotFound);

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Attachment URL is required.");

            return new TicketAttachments
            {
                TicketId = ticketId,
                URL = url.Trim()
            };
        }
    }
}
