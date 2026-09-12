using Domain.Enum;

namespace Domain.Entities
{
    public class Ticket
    {
        private readonly List<TicketHistory> _ticketHistories = new();
        private readonly List<TicketAttachments> _attachmentUrl = new();

        private Ticket()
        {
        }

        public int TicketId { get; private set; }
        public DateOnly? DueTo { get; private set; }
        public string? Description { get; private set; }
        public TicketPriority Priority { get; private set; }
        public string TicketTitle { get; private set; } = string.Empty;
        public TicketStatus TicketStatus { get; private set; } = TicketStatus.Pending;
        public DateOnly CreatedAt { get; private set; } = DateOnly.FromDateTime(DateTime.Now);

        // RelationShips for EF_Core
        // Project
        public int ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;

        // Current assigned Employee
        public int? EmployeeId { get; private set; }
        public Employee? Employee { get; private set; }

        // Employee who created the ticket
        public int TicketCreatedById { get; private set; }
        public Employee TicketCreatedBy { get; private set; } = null!;

        // Ticket History
        public IReadOnlyCollection<TicketHistory> TicketHistories => _ticketHistories;

        // Ticket Attachments
        public IReadOnlyCollection<TicketAttachments> AttachmentURL => _attachmentUrl;

        public static Ticket Create(
            string ticketTitle,
            DateOnly? dueTo,
            string? description,
            TicketPriority priority,
            int projectId,
            int employeeId,
            int ticketCreatedById)
        {
            if (employeeId <= 0 || ticketCreatedById <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (projectId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            var ticket = new Ticket
            {
                Priority = priority,
                ProjectId = projectId,
                EmployeeId = employeeId,
                TicketCreatedById = ticketCreatedById,
                TicketStatus = TicketStatus.Pending
            };

            ticket.UpdateDetails(ticketTitle, dueTo, description, employeeId);
            return ticket;
        }

        public void UpdateDetails(
            string ticketTitle,
            DateOnly? dueTo,
            string? description,
            int employeeId)
        {
            if (string.IsNullOrWhiteSpace(ticketTitle))
                throw new ArgumentException(ErrorShared.Ticket.TicketTitleRequired);

            if (employeeId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            TicketTitle = ticketTitle.Trim();
            DueTo = dueTo;
            Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();
            EmployeeId = employeeId;
        }

        public void ChangeStatus(TicketStatus status)
        {
            if (!System.Enum.IsDefined(status))
                throw new ArgumentException(ErrorShared.Ticket.InvalidStatus);

            TicketStatus = status;
        }

        public void SubmitForReview()
        {
            TicketStatus = TicketStatus.NeedReview;
        }

        public void Approve()
        {
            TicketStatus = TicketStatus.Done;
        }

        public void RequestChanges()
        {
            TicketStatus = TicketStatus.InProgress;
        }

        public void Reassign(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            EmployeeId = employeeId;
        }

        public void UnassignAndResetToPending()
        {
            EmployeeId = null;
            Employee = null;
            TicketStatus = TicketStatus.Pending;
        }

        public void ChangePriority(TicketPriority priority)
        {
            if (!System.Enum.IsDefined(priority))
                throw new ArgumentException(ErrorShared.Ticket.InvalidPriority);

            Priority = priority;
        }

        public override string ToString() =>
            $"{TicketId} Ticket Title: {TicketTitle} Created On: {CreatedAt}\nTicket Status: {TicketStatus}";
    }
}
