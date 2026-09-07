using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetAllTicketsForAProjectAsync(int projectId);
        Task<IEnumerable<Ticket>> GetAllTicketsForAnEmployeeAsync(int employeeId);

        Task<int> GetTicketTotalCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketInProgressCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketCompletedCountForAnEmployeeAsync(int employeeId);

        Task ChangeTicketStatusAsync(int ticketId, TicketStatus status);
        Task ChangeTicketPriorityAsync(int ticketId, TicketPriority priority);

        Task CreateAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(Ticket ticket);

        Task AddAttachmentToTicketAsync(int ticketId, string filePath);

        Task<bool> TicketExistsAsync(int ticketId);
        Task<bool> EmployeeExistsAsync(int employeeId);
        Task<bool> ProjectExistsAsync(int projectId);
        Task<bool> IsManagerAsync(int employeeId, int projectId);
    }
}
