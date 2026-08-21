using ApplicationServices.DTOs.Project;

namespace ApplicationServices.Interfaces
{
    public interface IProjectManager
    {
        IEnumerable<ProjectResponse> GetAllProjectWorkedByEmployee(int employeeId);
        IEnumerable<string[]> GetAllProjectWorkedByEmployeeTopThree(int employeeId);
        int GetProjectCount(int employeeId);

        ProjectResponse Create(CreateProjectRequest request);
        ProjectResponse? GetById(int id);
        ProjectResponse Update(int id, UpdateProjectRequest request);
        bool Delete(int id);

        void SetProjectAsActive(int projectId);
        void SetProjectAsCancelled(int projectId);
        void SetProjectAsCompleted(int projectId);
        void SetProjectAsOnHold(int projectId);


        //IEnumerable<ProjectResponse> GetAll();

        //IEnumerable<EmployeeResponse> GetEmployees(int projectId);

        //IEnumerable<TicketResponse> GetTickets(int projectId);

        //TicketResponse? GetTicket(int projectId, int TicketId);
    }
}