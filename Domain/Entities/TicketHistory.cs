namespace Domain.Entities
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int? ToEmployeeId { get; set; } // New assigned employee
        public int? FromEmployeeId { get; set; } // Previous assigned employee
        public Employee? ToEmployee { get; set; }
        public Ticket Ticket { get; set; } = null!;
        public int ActionByEmployeeId { get; set; } // Employee who performed the action
        public Employee? FromEmployee { get; set; }
        public string Action { get; set; } = null!;
        public Employee ActionByEmployee { get; set; } = null!;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}