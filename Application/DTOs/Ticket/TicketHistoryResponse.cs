namespace ApplicationServices.DTOs.Ticket
{
    public class TicketHistoryResponse
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? Note { get; set; }

        public int ActionByEmployeeId { get; set; }

        public string? ActionByEmployeeName { get; set; }

        public int? FromEmployeeId { get; set; }

        public string? FromEmployeeName { get; set; }

        public int? ToEmployeeId { get; set; }

        public string? ToEmployeeName { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
