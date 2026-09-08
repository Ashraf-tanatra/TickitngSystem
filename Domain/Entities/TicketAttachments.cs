namespace Domain.Entities
{
    public class TicketAttachments : BaseEntity
    {
        private TicketAttachments()
        {
        }

        public string URL { get; private set; } = string.Empty;
        public string OriginalFileName { get; private set; } = string.Empty;
        public string StoredFileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long SizeInBytes { get; private set; }
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
                URL = url.Trim(),
                OriginalFileName = Path.GetFileName(url.Trim()),
                StoredFileName = Path.GetFileName(url.Trim()),
                ContentType = "application/octet-stream"
            };
        }

        public static TicketAttachments Create(
            int ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes)
        {
            if (ticketId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.TicketNotFound);

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Attachment URL is required.");

            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("Original file name is required.");

            if (string.IsNullOrWhiteSpace(storedFileName))
                throw new ArgumentException("Stored file name is required.");

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type is required.");

            if (sizeInBytes <= 0)
                throw new ArgumentException("Attachment size must be greater than zero.");

            return new TicketAttachments
            {
                TicketId = ticketId,
                URL = url.Trim(),
                OriginalFileName = Path.GetFileName(originalFileName.Trim()),
                StoredFileName = Path.GetFileName(storedFileName.Trim()),
                ContentType = contentType.Trim(),
                SizeInBytes = sizeInBytes
            };
        }
    }
}
