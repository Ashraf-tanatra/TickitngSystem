using ApplicationServices.DTOs.Ticket;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        bool Delete(int id);
        TicketResponse? GetById(int id);
        int Create(CreateTicketRequest request);
        void Update(int id, UpdateTicketRequest request);
        int GetTicketTotalCountForAnEmployee(int employeeId);
        int GetTicketCompletedCountForAnEmployee(int employeeId);
        int GetTicketInProgressCountForAnEmployee(int employeeId);
        void AddAttachmentToTicket(int ticketId, string filePath);
        void ChangeTicketStatus(int ticketId, TicketStatus status);
        void ChangeTicketPriority(int ticketId, TicketPriority priority);
        IEnumerable<TicketResponse>? GetAllTicketsForAProject(int projectId);
        IEnumerable<TicketResponse>? GetAllTicketsForAnEmployee(int employeeId);

        bool TicketExists(int ticketId);
        bool EmployeeExists(int employeeId);
        bool ProjectExists(int projectId);
    }
}