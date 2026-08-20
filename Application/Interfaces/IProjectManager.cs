using ApplicationServices.DTOs.Project;
using Domain.Entities;

namespace ApplicationServices.Interfaces
{
    public interface IProjectManager
    {
        IEnumerable<Project> GetAllProjectWorkedByEmployee(int employeeId);
        IEnumerable<Project> GetAllProjectWorkedByEmployeeTopThree(int employeeId);
        ProjectResponse Create(CreateProjectRequest request);
        ProjectResponse? GetById(int id);
        ProjectResponse Update(int id, UpdateProjectRequest request);
        bool Delete(int id);


        //IEnumerable<ProjectResponse> GetAll();

        //IEnumerable<EmployeeResponse> GetEmployees(int projectId);

        //IEnumerable<TicketResponse> GetTickets(int projectId);

        //TicketResponse? GetTicket(int projectId, int TicketId);
    }
}