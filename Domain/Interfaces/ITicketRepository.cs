using Domain.Entities;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        Ticket? GetById(int id);
        IEnumerable<Ticket> GetAllTicketsForAProject(int projectId);
        IEnumerable<Ticket> GetAllTicketsForAnEmployee(int employeeId);

        int GetTicketTotalCountForAnEmployee(int employeeId);
        int GetTicketInProgressCountForAnEmployee(int employeeId);
        int GetTicketCompletedCountForAnEmployee(int employeeId);

        void ChangeTicketStatus(int ticketId, TicketStatus status);
        void ChangeTicketPriority(int ticketId, TicketPriority priority);

        void Add(Ticket ticket);
        void Update(Ticket ticket);
        void Delete(Ticket ticket);

        bool EmployeeExists(int employeeId);
        bool ProjectExists(int projectId);


        //void SetStatusToInProgress(int ticketId);
        //void SetStatusToPending(int ticketId);
        //void SetStatusToCompleted(int ticketId);
        //void SetStatusToReOpen(int ticketId);
        //void SetStatusToDone(int ticketId);
        //void SetStatusToCancelled(int ticketId);

        //void SetPriorityToLow(int ticketId);
        //void SetPriorityToMedium(int ticketId);
        //void SetPriorityToHigh(int ticketId);



        //IEnumerable<Ticket> GetAll();
    }
}