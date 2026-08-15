using ApplicationServices.DTOs;

namespace Domain.EntityManager
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
    }
}