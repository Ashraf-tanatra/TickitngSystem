using ApplicationServices.DTOs.Project;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface IProjectManager
    {
        IEnumerable<ProjectResponse>? GetAllProjectWorkedByEmployee(int employeeId);
        IEnumerable<string[]>? GetAllProjectWorkedByEmployeeTopThree(int employeeId);
        IEnumerable<EmployeeResponse>? GetEmployeesWorkOnProject(int projectId);
        int GetProjectCount(int employeeId);

        int Create(CreateProjectRequest request);
        ProjectResponse? GetById(int id);
        ProjectResponse Update(int id, UpdateProjectRequest request);
        bool Delete(int id);

        void ProjectAddEmployee(ProjectEmployeeRequest request);

        void SetProjectAsActive(int projectId);
        void SetProjectAsCancelled(int projectId);
        void SetProjectAsCompleted(int projectId);
        void SetProjectAsOnHold(int projectId);


        IEnumerable<ProjectResponse>? GetAllProjectWorkedByEmployeeWithFilter(int employeeId, ProjectStatus FilterByStatus);

        //IEnumerable<TicketResponse> GetTickets(int projectId);

        //TicketResponse? GetTicket(int projectId, int TicketId);
    }
}