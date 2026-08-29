using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetById(int id);
        Task<bool> Create(Ticket ticket);
        Task<bool> Update(Ticket ticket);
        Task<bool> Delete(int ticketId);
        Task<int> GetTicketTotalCountForAnEmployee(int employeeId);
        Task<int> GetTicketCompletedCountForAnEmployee(int employeeId);
        Task<int> GetTicketInProgressCountForAnEmployee(int employeeId);
        Task<bool> AddAttachmentToTicket(int ticketId, string filePath);
        Task<bool> ChangeTicketStatus(int ticketId, TicketStatus status);
        Task<IEnumerable<Ticket>> GetAllTicketsForAProject(int projectId);
        Task<IEnumerable<Ticket>> GetAllTicketsForAnEmployee(int employeeId);
        Task<bool> ChangeTicketPriority(int ticketId, TicketPriority priority);

        Task<bool> TicketExists(int ticketId);
        Task<string?> GetEmpName(int ticketId);
        Task<bool> ProjectExists(int projectId);
        Task<bool> EmployeeExists(int employeeId);
        Task<bool> IsManager(int employeeId, int projectId);
    }
}