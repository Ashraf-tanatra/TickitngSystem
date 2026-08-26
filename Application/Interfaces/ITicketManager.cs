using ApplicationServices.DTOs.Ticket;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        Task<bool> Delete(int id);
        Task<TicketResponse?> GetById(int id);
        Task<int> Create(CreateTicketRequest request);
        Task<bool> Update(int id, UpdateTicketRequest request);
        Task<int> GetTicketTotalCountForAnEmployee(int employeeId);
        Task<int> GetTicketCompletedCountForAnEmployee(int employeeId);
        Task<int> GetTicketInProgressCountForAnEmployee(int employeeId);
        Task<bool> AddAttachmentToTicket(int ticketId, string filePath);
        Task<bool> ChangeTicketStatus(int ticketId, TicketStatus status);
        Task<bool> ChangeTicketPriority(int ticketId, TicketPriority priority);
        Task<IEnumerable<TicketResponse>?> GetAllTicketsForAProject(int projectId);
        Task<IEnumerable<TicketResponse>?> GetAllTicketsForAnEmployee(int employeeId);

        Task<bool> TicketExists(int ticketId);
        Task<bool> EmployeeExists(int employeeId);
        Task<bool> ProjectExists(int projectId);
    }
}