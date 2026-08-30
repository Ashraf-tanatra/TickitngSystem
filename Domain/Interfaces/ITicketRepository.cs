using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        Ticket? GetById(int id);
        IEnumerable<Ticket>? GetAllTicketsForAProject(int projectId);
        IEnumerable<Ticket>? GetAllTicketsForAnEmployee(int employeeId);

        int GetTicketTotalCountForAnEmployee(int employeeId);
        int GetTicketInProgressCountForAnEmployee(int employeeId);
        int GetTicketCompletedCountForAnEmployee(int employeeId);

        void ChangeTicketStatus(int ticketId, TicketStatus status);
        void ChangeTicketPriority(int ticketId, TicketPriority priority);

        void Create(Ticket ticket);
        void Update(Ticket ticket);
        void Delete(Ticket ticket);

        void AddAttachmentToTicket(int ticketId, string filePath);

        bool TicketExists(int ticketId);
        bool EmployeeExists(int employeeId);
        bool ProjectExists(int projectId);
        bool IsManager(int employeeId, int projectId);
    }
}