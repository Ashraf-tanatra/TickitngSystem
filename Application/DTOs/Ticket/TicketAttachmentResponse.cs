namespace ApplicationServices.DTOs.Ticket
{
    public class TicketAttachmentResponse
    {
        public string OriginalFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long SizeInBytes { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}
