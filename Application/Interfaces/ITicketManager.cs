using ApplicationServices.DTOs.Ticket;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        TicketResponse? GetById(int id);

        IEnumerable<TicketResponse>? GetAllTicketsForAProject(int projectId);
        IEnumerable<TicketResponse>? GetAllTicketsForAnEmployee(int employeeId);

        int Create(CreateTicketRequest request);
        void Update(int id, UpdateTicketRequest request);
        bool Delete(int id);


        int GetTicketTotalCountForAnEmployee(int employeeId);
        int GetTicketInProgressCountForAnEmployee(int employeeId);
        int GetTicketCompletedCountForAnEmployee(int employeeId);

        void ChangeTicketStatus(int ticketId, TicketStatus status);
        void ChangeTicketPriority(int ticketId, TicketPriority priority);

        void AddAttachmentToTicket(int ticketId, string filePath);

        bool TicketExists(int ticketId);
        bool EmployeeExists(int employeeId);
        bool ProjectExists(int projectId);
    }
}