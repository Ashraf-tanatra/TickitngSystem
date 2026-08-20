using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class ProjectManager : IProjectManager
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectManager(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public IEnumerable<Project> GetAllProjectWorkedByEmployee(int employeeId)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Project> GetAllProjectWorkedByEmployeeTopThree(int employeeId)
        {
            throw new NotImplementedException();
        }

        // GET By ID
        public ProjectResponse? GetById(int id)
        {
            var project = _projectRepository.GetById(id);

            if (project == null)
                return null;

            return new ProjectResponse
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectManagerId = project.ProjectManagerId, // ?

                ProjectManagerName = project.ProjectManager == null
                    ? null
                    : $"{project.ProjectManager.FName} {project.ProjectManager.LName}",

                EmployeeCount = project.ProjectEmployees.Count,   // ?
                TicketCount = project.ProjectTickets.Count        // ?
            };
        }
        // CREATE
        public ProjectResponse Create(CreateProjectRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.ProjectName))
                throw new ArgumentException(
                    "Project name is required.");

            if (!_projectRepository.EmployeeExists(
                    request.ProjectManagerId))
            {
                throw new ArgumentException(
                    "The specified Project Manager does not exist.");
            }

            //if (!_projectRepository.IsManager(
            //        request.ProjectManagerId))
            //{
            //    throw new ArgumentException(
            //        "The specified employee is not a Project Manager.");
            //}

            var project = new Project
            {
                ProjectName = request.ProjectName,
                ProjectDescription = request.ProjectDescription,
                ProjectManagerId = request.ProjectManagerId
            };

            _projectRepository.Create(project);

            return GetById(project.Id)!;
        }
        // UPDATE
        public ProjectResponse Update(int id, UpdateProjectRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var project = _projectRepository.GetById(id);

            if (project == null)
                throw new KeyNotFoundException(
                    "Project not found.");

            if (string.IsNullOrWhiteSpace(request.ProjectName))
                throw new ArgumentException(
                    "Project name is required.");

            if (!_projectRepository.EmployeeExists(
                    request.ProjectManagerId))
            {
                throw new ArgumentException(
                    "The specified Project Manager does not exist.");
            }

            //if (!_projectRepository.IsManager(
            //        request.ProjectManagerId))
            //{
            //    throw new ArgumentException(
            //        "The specified employee is not a Project Manager.");
            //}

            project.ProjectName = request.ProjectName;
            project.ProjectDescription = request.ProjectDescription;
            project.ProjectManagerId = request.ProjectManagerId;

            _projectRepository.Update(project);

            return GetById(project.Id)!;
        }
        // Delete
        public bool Delete(int id)
        {
            var project = _projectRepository.GetById(id);

            if (project == null)
                return false;

            _projectRepository.Delete(project);

            return true;
        }


        // GET EMPLOYEES OF PROJECT
        //public IEnumerable<EmployeeResponse> GetEmployees(int projectId) // get emp that works in this project? 
        //{
        //    var employees = _projectRepository.GetEmployees(projectId);

        //    return employees.Select(employee => new EmployeeResponse
        //    {
        //        Id = employee.Id,
        //        FName = employee.FName,
        //        LName = employee.LName,
        //        Phone = employee.Phone,
        //        Gender = employee.Gender,
        //        IsDeleted = employee.IsDeleted
        //    });
        //}

        //public IEnumerable<ProjectResponse> GetAll() // ?
        //{
        //    var projects = _projectRepository.GetAll();

        //    return projects.Select(project => new ProjectResponse
        //    {
        //        Id = project.Id,
        //        ProjectName = project.ProjectName,
        //        ProjectDescription = project.ProjectDescription,
        //        ProjectManagerId = project.ProjectManagerId,

        //        ProjectManagerName = project.ProjectManager == null
        //            ? null
        //            : $"{project.ProjectManager.FName} {project.ProjectManager.LName}",

        //        EmployeeCount = project.ProjectEmployees.Count,
        //        TicketCount = project.ProjectTickets.Count
        //    });
        //}

        //GET TICKETS
        //public IEnumerable<TicketResponse> GetTickets(int projectId)
        //{
        //    var tickets = _projectRepository.GetTickets(projectId);

        //    return tickets.Select(ticket => new TicketResponse
        //    {
        //        TicketId = ticket.TicketId,
        //        TicketTitle = ticket.TicketTitle,
        //        DueTo = ticket.DueTo,
        //        TicketStatus = ticket.TicketStatus.ToString(),
        //        Priority = ticket.Priority.ToString(),
        //        Description = ticket.Description,
        //        EmployeeId = ticket.EmployeeId,
        //        ProjectId = ticket.ProjectId
        //    });
        //}

        // GET TICKET BY ID
        //public TicketResponse? GetTicket(int projectId, int ticketId)
        //{
        //    var tickets = _projectRepository.GetTickets(projectId);

        //    var ticket = tickets.FirstOrDefault(t => t.TicketId == ticketId); // ?

        //    if (ticket == null)
        //        return null;

        //    return new TicketResponse
        //    {
        //        TicketId = ticket.TicketId,
        //        TicketTitle = ticket.TicketTitle,
        //        DueTo = ticket.DueTo,
        //        TicketStatus = ticket.TicketStatus.ToString(),
        //        Priority = ticket.Priority.ToString(),
        //        Description = ticket.Description,
        //        EmployeeId = ticket.EmployeeId,
        //        ProjectId = ticket.ProjectId
        //    };
        //}

    }
}