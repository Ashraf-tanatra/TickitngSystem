namespace Domain.Entities
{
    public class TicketHistory : BaseEntity
    {
        private TicketHistory()
        {
        }

        // Ticket
        public int TicketId { get; private set; }
        public Ticket Ticket { get; private set; } = null!;

        // Employee who performed the action
        public int ActionByEmployeeId { get; private set; }
        public Employee ActionByEmployee { get; private set; } = null!;

        // Previous assigned employee
        public int? FromEmployeeId { get; private set; }
        public Employee? FromEmployee { get; private set; }

        // New assigned employee
        public int? ToEmployeeId { get; private set; }
        public Employee? ToEmployee { get; private set; }

        // What happened
        public string Action { get; private set; } = string.Empty;

        // Previous value
        public string? OldValue { get; private set; }

        // New value
        public string? NewValue { get; private set; }

        public string? Note { get; private set; }

        // When it happened
        public DateTime ModifiedAt { get; private set; } = DateTime.Now;

        public static TicketHistory Create(
            int ticketId,
            int actionByEmployeeId,
            string action,
            string? oldValue = null,
            string? newValue = null,
            int? fromEmployeeId = null,
            int? toEmployeeId = null,
            string? note = null)
        {
            if (ticketId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.TicketNotFound);

            if (actionByEmployeeId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException(ErrorShared.Ticket.HistoryActionRequired);

            return new TicketHistory
            {
                TicketId = ticketId,
                ActionByEmployeeId = actionByEmployeeId,
                Action = action.Trim(),
                OldValue = string.IsNullOrWhiteSpace(oldValue) ? null : oldValue.Trim(),
                NewValue = string.IsNullOrWhiteSpace(newValue) ? null : newValue.Trim(),
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = toEmployeeId,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
                ModifiedAt = DateTime.Now
            };
        }
    }
}
