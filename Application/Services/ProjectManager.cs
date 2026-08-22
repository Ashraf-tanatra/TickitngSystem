using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enum;
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

        public IEnumerable<ProjectResponse>? GetAllProjectWorkedByEmployee(int employeeId)
        {

            var Emp = _projectRepository.EmployeeExists(employeeId);
            if (!Emp)
                return null;

            var project = _projectRepository.GetAllProjectWorkedByEmployee(employeeId);
            if (project == null)
                return null;

            return project.Select(project => new ProjectResponse
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectStatus = project.ProjectStatus.ToString(),
                ProjectManagerId = project.ProjectManagerId,
                StartDate = project.StartedAt,
                EndDate = project.EndAt,
                EmployeeRole = (project.ProjectManagerId == employeeId ? "Manager" : null)
                ?? project.ProjectEmployees?.FirstOrDefault(pe => pe.EmployeeId == employeeId)?.Role


                //ProjectManagerName = project.ProjectManager == null ? null
                //    : $"{project.ProjectManager.FName} {project.ProjectManager.LName}"

                //EmployeeCount = project.ProjectEmployees.Count,
                //TicketCount = project.ProjectTickets.Count
            });
        }
        public IEnumerable<string[]>? GetAllProjectWorkedByEmployeeTopThree(int employeeId)
            => _projectRepository.GetAllProjectWorkedByEmployeeTopThree(employeeId);
        public IEnumerable<EmployeeResponse>? GetEmployeesWorkOnProject(int projectId)
        {
            var employees = _projectRepository.GetEmployees(projectId);
            return employees?.Select(employee => new EmployeeResponse
            {
                Id = employee.Id,
                FName = employee.FName,
                LName = employee.LName,
                Phone = employee.Phone,
                Gender = employee.Gender
            });
        }
        public int GetProjectCount(int employeeId)
        {
            return _projectRepository.GetProjectCount(employeeId);
        }
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
                ProjectStatus = project.ProjectStatus.ToString(),
                ProjectManagerId = project.ProjectManagerId,
                StartDate = project.StartedAt,
                EndDate = project.EndAt,

                EmployeeRole = (project.ProjectManagerId == id ? "Manager" : null)
                ?? project.ProjectEmployees?.FirstOrDefault(pe => pe.EmployeeId == id)?.Role,

                ProjectManagerName = project.ProjectManager == null
                    ? null : $"{project.ProjectManager.FName} {project.ProjectManager.LName}",
            };
        }

        public int Create(CreateProjectRequest request)
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

            var project = new Project
            {
                ProjectName = request.ProjectName,
                ProjectDescription = request.ProjectDescription,
                ProjectManagerId = request.ProjectManagerId,
                StartedAt = request.StartTime,
                EndAt = request.EndTime
            };

            _projectRepository.Create(project);

            return project.Id;
            // return GetById(project.Id); // may need edit

            //if (!_projectRepository.IsManager(
            //        request.ProjectManagerId))
            //{
            //    throw new ArgumentException(
            //        "The specified employee is not a Project Manager.");
            //}
        }

        // need edit for testing for project manager only can edit
        public void Update(int projectId, int empId, UpdateProjectRequest request)
        {
            Exception exception = new Exception();

            if (request == null)
                throw exception;

            if (!_projectRepository.ProjectExits(projectId))
                throw exception;

            if (string.IsNullOrWhiteSpace(request.ProjectName))
                throw exception;

            if (!_projectRepository.EmployeeExists(empId))
                throw exception;

            if (!_projectRepository.IsManager(projectId, empId))
                throw exception;

            var project = _projectRepository.GetById(projectId);

            project.ProjectName = request.ProjectName;
            project.ProjectDescription = request.ProjectDescription;
            //project.ProjectManagerId = request.ProjectManagerId;
            project.StartedAt = request.StartDate;
            project.EndAt = request.EndDate;

            _projectRepository.Update(project);
        }
        public void Delete(int id, int empId) => _projectRepository.Delete(id, empId);

        public void ProjectAddEmployee(ProjectEmployeeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (!_projectRepository.EmployeeExists(request.EmployeeId))
                throw new ArgumentException(
                    "The specified employee does not exist.");

            var projectEmployee = new ProjectEmployee
            {
                EmployeeId = request.EmployeeId,
                ProjectId = request.ProjectId,
                Role = request.Role
            };
            _projectRepository.AddEmployeeToProject(projectEmployee);
        }

        void IProjectManager.SetProjectStatus(int projectId, ProjectStatus status)
        {
            if ((int)status < 0 || (int)status > 4)
                throw new ArgumentException("Invalid project status.");

            _projectRepository.SetProjectStatus(projectId, status);
        }

        public bool ProjectExits(int projectId)
        {
            return _projectRepository.ProjectExits(projectId);
        }

        public IEnumerable<ProjectResponse>? GetAllProjectWorkedByEmployeeWithFilter(int employeeId, ProjectStatus FilterByStatus)
        {
            var Emp = _projectRepository.EmployeeExists(employeeId);
            if (!Emp)
                return null;

            var project = _projectRepository.GetAllProjectWorkedByEmployeeWithFilter(employeeId, FilterByStatus);

            if (project == null)
                return null;

            return project.Select(project => new ProjectResponse
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectStatus = project.ProjectStatus.ToString(),
                ProjectManagerId = project.ProjectManagerId,
                StartDate = project.StartedAt,
                EndDate = project.EndAt,

                EmployeeRole = (project.ProjectManagerId == employeeId ? "Manager" : null)
                ?? project.ProjectEmployees?.FirstOrDefault(pe => pe.EmployeeId == employeeId)?.Role
            });
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