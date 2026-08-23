using Domain.Entities;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        Ticket? GetById(int id);

        IEnumerable<Ticket> GetAllTicketsForAProject(int projectId);

        IEnumerable<Ticket> GetAllTicketsForAnEmployee(int employeeId);

        int GetTicketTotalCountForAnEmployee(int employeeId);

        int GetTicketInProgressCountForAnEmployee(int employeeId);

        int GetTicketCompletedCountForAnEmployee(int employeeId);


        void Add(Ticket ticket);
        void Delete(Ticket ticket);
        void Update(Ticket ticket);
        bool ProjectExists(int projectId);
        bool EmployeeExists(int employeeId);
        void ChangeTicketPriority(int ticketId, TicketPriority priority);
        void ReassignTicket(int ticketId,int toEmployeeId,int actionByEmployeeId);
        void ChangeTicketStatus(int ticketId, TicketStatus status, int actionByEmployeeId);
    }
}