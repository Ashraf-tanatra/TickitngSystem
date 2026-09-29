namespace ApplicationServices.DTOs.Ticket
{
    public class TicketHistoryResponse
    {
        public string Action { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? Note { get; set; }

        public string? ActionByEmployeeName { get; set; }

        public string? FromEmployeeName { get; set; }

        public string? ToEmployeeName { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
