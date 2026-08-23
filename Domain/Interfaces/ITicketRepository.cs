using Domain.Entities;
using Domain.Enum;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        // GET
        Ticket? GetById(int id);

        IEnumerable<Ticket> GetAllTicketsForAProject(
            int projectId);

        IEnumerable<Ticket> GetAllTicketsForAnEmployee(
            int employeeId);

        // COUNTS
        int GetTicketTotalCountForAnEmployee(
            int employeeId);

        int GetTicketInProgressCountForAnEmployee(
            int employeeId);

        int GetTicketCompletedCountForAnEmployee(
            int employeeId);

        // UPDATE TICKET STATUS / PRIORITY
        void ChangeTicketStatus(int ticketId, TicketStatus status, int actionByEmployeeId);

        void ChangeTicketPriority(
            int ticketId,
            TicketPriority priority);
        // Reassign Ticket
        void ReassignTicket(
    int ticketId,
    int fromEmployeeId,
    int toEmployeeId,
    int actionByEmployeeId);

        // CRUD
        void Add(Ticket ticket);

        void Update(Ticket ticket);

        void Delete(Ticket ticket);

        // VALIDATION
        bool EmployeeExists(int employeeId);

        bool ProjectExists(int projectId);

        bool EmployeeBelongsToProject(
            int employeeId,
            int projectId);
    }
}