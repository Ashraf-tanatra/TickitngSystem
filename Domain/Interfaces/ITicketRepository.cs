using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<IEnumerable<TicketHistory>> GetTicketHistoryAsync(Guid ticketId);
        Task<IEnumerable<TicketAttachments>> GetTicketAttachmentsAsync(Guid ticketId);
        Task<IEnumerable<Ticket>> GetAllTicketsForAProjectAsync(Guid projectId);
        Task<IEnumerable<Ticket>> GetAllTicketsForAnEmployeeAsync(Guid employeeId);

        Task<int> GetTicketTotalCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketInProgressCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketCompletedCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketNeedReviewCountForAnEmployeeAsync(Guid employeeId);

        Task ChangeTicketStatusAsync(Guid ticketId, TicketStatus status);
        Task ChangeTicketPriorityAsync(Guid ticketId, TicketPriority priority);

        Task CreateAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task UpdateWithHistoryAsync(Ticket ticket, TicketHistory history);
        Task DeleteAsync(Ticket ticket);

        Task AddAttachmentToTicketAsync(
            Guid ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes);

        Task<bool> TicketExistsAsync(Guid ticketId);
        Task<bool> EmployeeExistsAsync(Guid employeeId);
        Task<bool> ProjectExistsAsync(Guid projectId);
        Task<bool> IsManagerAsync(Guid employeeId, Guid projectId);
        Task<bool> IsEmployeeAssignedToProjectAsync(Guid employeeId, Guid projectId);
    }
}
