using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        Ticket? GetById(int id);
        void Create(Ticket ticket);
        void Update(Ticket ticket);
        void Delete(Ticket ticket);
        int GetTicketTotalCountForAnEmployee(int employeeId);
        int GetTicketCompletedCountForAnEmployee(int employeeId);
        int GetTicketInProgressCountForAnEmployee(int employeeId);
        void AddAttachmentToTicket(int ticketId, string filePath);
        void ChangeTicketStatus(int ticketId, TicketStatus status);
        IEnumerable<Ticket>? GetAllTicketsForAProject(int projectId);
        IEnumerable<Ticket>? GetAllTicketsForAnEmployee(int employeeId);
        void ChangeTicketPriority(int ticketId, TicketPriority priority);

        bool TicketExists(int ticketId);
        bool EmployeeExists(int employeeId);
        bool ProjectExists(int projectId);
        bool IsManager(int employeeId, int projectId);
    }
}