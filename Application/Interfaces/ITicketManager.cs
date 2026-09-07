using ApplicationServices.DTOs.Ticket;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        Task<TicketResponse?> GetByIdAsync(int id);

        Task<IEnumerable<TicketResponse>> GetAllTicketsForAProjectAsync(int projectId);
        Task<IEnumerable<TicketResponse>> GetAllTicketsForAnEmployeeAsync(int employeeId);

        Task<int> CreateAsync(CreateTicketRequest request);
        Task UpdateAsync(int id, UpdateTicketRequest request);
        Task<bool> DeleteAsync(int id);


        Task<int> GetTicketTotalCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketInProgressCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketCompletedCountForAnEmployeeAsync(int employeeId);

        Task ChangeTicketStatusAsync(int ticketId, TicketStatus status);
        Task ChangeTicketPriorityAsync(int ticketId, TicketPriority priority);

        Task AddAttachmentToTicketAsync(int ticketId, string filePath);

        Task<bool> TicketExistsAsync(int ticketId);
        Task<bool> EmployeeExistsAsync(int employeeId);
        Task<bool> ProjectExistsAsync(int projectId);
    }
}
