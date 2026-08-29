using ApplicationServices.DTOs.Ticket;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        Task<TicketResponse?> GetById(int id);
        Task<bool> Delete(int empId, int ticketId);
        Task<int> Create(CreateTicketRequest request, int empCreatedById);
        Task<bool> Update(int id, int empId, UpdateTicketRequest request);
        Task<int> GetTicketTotalCountForAnEmployee(int employeeId);
        Task<int> GetTicketCompletedCountForAnEmployee(int employeeId);
        Task<int> GetTicketInProgressCountForAnEmployee(int employeeId);
        Task<bool> AddAttachmentToTicket(int ticketId, string filePath);
        Task<bool> ChangeTicketStatus(int ticketId, int empId, TicketStatus status);
        Task<bool> ChangeTicketPriority(int ticketId, int empCreatedById, TicketPriority priority);
        Task<IEnumerable<TicketResponse>?> GetAllTicketsForAProject(int projectId);
        Task<IEnumerable<TicketResponse>?> GetAllTicketsForAnEmployee(int employeeId);

        Task<bool> TicketExists(int ticketId);

    }
}