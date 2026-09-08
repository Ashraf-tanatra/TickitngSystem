using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(int id);
        Task<IEnumerable<TicketHistory>> GetTicketHistoryAsync(int ticketId);
        Task<IEnumerable<TicketAttachments>> GetTicketAttachmentsAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetAllTicketsForAProjectAsync(int projectId);
        Task<IEnumerable<Ticket>> GetAllTicketsForAnEmployeeAsync(int employeeId);

        Task<int> GetTicketTotalCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketInProgressCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketCompletedCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketNeedReviewCountForAnEmployeeAsync(int employeeId);

        Task ChangeTicketStatusAsync(int ticketId, TicketStatus status);
        Task ChangeTicketPriorityAsync(int ticketId, TicketPriority priority);

        Task CreateAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task UpdateWithHistoryAsync(Ticket ticket, TicketHistory history);
        Task DeleteAsync(Ticket ticket);

        Task AddAttachmentToTicketAsync(
            int ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes);

        Task<bool> TicketExistsAsync(int ticketId);
        Task<bool> EmployeeExistsAsync(int employeeId);
        Task<bool> ProjectExistsAsync(int projectId);
        Task<bool> IsManagerAsync(int employeeId, int projectId);
        Task<bool> IsEmployeeAssignedToProjectAsync(int employeeId, int projectId);
    }
}
