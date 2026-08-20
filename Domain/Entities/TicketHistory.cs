namespace Domain.Entities
{
    public class TicketHistory
    {
        public int Id { get; set; }
        // Ticket
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;
        // Employee who performed the action
        public int ActionByEmployeeId { get; set; }
        public Employee ActionByEmployee { get; set; } = null!;
        // Previous assigned employee
        public int? FromEmployeeId { get; set; }
        public Employee? FromEmployee { get; set; }
        // New assigned employee
        public int? ToEmployeeId { get; set; }
        public Employee? ToEmployee { get; set; }
        // What happened
        public string Action { get; set; } = string.Empty; //?
        // Previous value
        public string? OldValue { get; set; }
        // New value
        public string? NewValue { get; set; }
        // When it happened
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}