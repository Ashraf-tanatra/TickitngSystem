using ApplicationServices.DTOs.Project;
using ApplicationServices.DTOs.Ticket;

namespace ApplicationServices.Interfaces
{
    public interface IProjectManager
    {
        ProjectResponse Create(CreateProjectRequest request);

        IEnumerable<ProjectResponse> GetAll();

        ProjectResponse? GetById(int id);

        ProjectResponse Update(
            int id,
            UpdateProjectRequest request);

        bool Delete(int id);

        IEnumerable<EmployeeResponse> GetEmployees(int projectId);

        IEnumerable<TicketResponse> GetTickets(int projectId);

        TicketResponse? GetTicket(int projectId , int TicketId);
    }
}