using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all the projects that the employee works on
        public IEnumerable<Project>? GetAllProjectWorkedByEmployee(int employeeId)
        {
            return _context.Projects
                    .Where(p => p.ProjectStatus != ProjectStatus.Cancelled)
                    .Include(p => p.ProjectEmployees)
                    .Where(p => p.ProjectManagerId == employeeId || p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                    .ToList();
        }
        // last three projects that the employee added to works on
        public IEnumerable<String[]>? GetAllProjectWorkedByEmployeeTopThree(int employeeId)
        {
            return _context.Projects
                   .Where(p => p.ProjectStatus == ProjectStatus.Active || p.ProjectStatus == ProjectStatus.OnHold)
                   .Include(p => p.ProjectEmployees)
                   .Where(p => p.ProjectManagerId == employeeId || p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                   .OrderByDescending(d => d.StartedAt)
                   .Take(3)
                   .Select(p => new string[2]
                   {
                        p.ProjectName,
                        p.ProjectEmployees!.FirstOrDefault(pe=>pe.EmployeeId==employeeId).Role
                   })
                   .ToList();
        }
        // All employees that work on the project
        public IEnumerable<Employee>? GetEmployees(int projectId)
        {
            return _context.ProjectEmployees
                .Where(pe => pe.ProjectId == projectId)
                .Select(pe => pe.Employee)
                .Where(e => !e.IsDeleted)
                .ToList();
        }
        //Number of projects that the employee works on
        public int GetProjectCount(int employeeId)
        {
            return _context.Projects
                   .Where(p => p.ProjectStatus == ProjectStatus.Active)
                   .Include(p => p.ProjectEmployees)
                   .Where(p => p.ProjectManagerId == employeeId || p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                   .Count();
        }
        public Project GetById(int id)
        {
            if (!ProjectExits(id))
                return null;

            return _context.Projects
                .Include(p => p.ProjectEmployees)!
                .ThenInclude(pe => pe.Employee)
                .FirstOrDefault(p => p.Id == id)!;
        }

        // Create
        public void Create(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }
        // Update
        public void Update(Project project)
        {
            _context.Projects.Update(project);
            _context.SaveChanges();
        }

        // Delete
        public void Delete(int projectId, int employeeId)
        {
            if (!ProjectExits(projectId))
                throw new ArgumentException($"Project does not exist.");
            if (!IsManager(projectId, employeeId))
                throw new ArgumentException($"Project cannot delete by you.");

            if (TicketExists(projectId))
                _context.Projects.Where(p => p.Id == projectId)
                    .ExecuteUpdate(setter => setter.SetProperty(p => p.ProjectStatus, ProjectStatus.Cancelled));

            else
            {
                var project = GetById(projectId);
                if (project.ProjectStatus == ProjectStatus.Cancelled)
                    throw new ArgumentException($"Project is already cancelled.");

                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
        }

        public void AddEmployeeToProject(ProjectEmployee projectEmployee)
        {
            if (!EmployeeExists(projectEmployee.EmployeeId))
                throw new ArgumentException($"Employee does not exist.");

            if (!ProjectExits(projectEmployee.ProjectId))
                throw new ArgumentException($"Project does not exist.");

            var empIsAlreadyInProject = _context.ProjectEmployees
                .Any(pe => pe.ProjectId == projectEmployee.ProjectId && pe.EmployeeId == projectEmployee.EmployeeId);

            if (empIsAlreadyInProject)
                throw new ArgumentException($"Employee is already in the project.");

            _context.ProjectEmployees.Add(projectEmployee);
            _context.SaveChanges();
        }

        public void SetProjectStatus(int projectId, ProjectStatus status)
        {
            _context.Projects
              .Where(p => p.Id == projectId)
              .ExecuteUpdate(setter => setter.SetProperty(p => p.ProjectStatus, status));
        }

        public bool EmployeeExists(int employeeId) => _context.Employees.Any(e => e.Id == employeeId);
        public bool ProjectExits(int projectId) => _context.Projects.Any(p => p.Id == projectId);
        public bool TicketExists(int projectId) => _context.Tickets.Any(t => t.ProjectId == projectId);
        public bool IsManager(int projectId, int employeeId)
        {
            return _context.Projects
                .Where(p => p.Id == projectId)
                .Any(e => e.ProjectManagerId == employeeId);
        }
        public IEnumerable<Project>? GetAllProjectWorkedByEmployeeWithFilter(int employeeId, ProjectStatus FilterByStatus)
        {

            return _context.Projects
                     .Where(p => p.ProjectStatus == FilterByStatus)
                     .Include(p => p.ProjectEmployees)
                     .Where(p => p.ProjectManagerId == employeeId || p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                     .ToList();
        }

    }
}