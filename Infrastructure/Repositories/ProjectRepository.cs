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

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectEmployees)
                .ThenInclude(pe => pe.Employee)
                .Include(p => p.ProjectTickets)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        // Update
        public async Task<bool> UpdateAsync(Project project)
        {
            if (!await ProjectExistsAsync(project.Id))
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return true;
        }
        // Create
        public async Task<bool> CreateAsync(Project project)
        {
            if (!await EmployeeExistsAsync(project.ProjectManagerId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);
            if (await ProjectExistsAsync(project.Id))
                throw new ArgumentException(ErrorShared.Project.ProjectAlreadyExists);

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return true;
        }
        //Number of projects that the employee works on
        public async Task<int> GetProjectCountAsync(int employeeId)
        {
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            return await _context.Projects
                        .Where(p => p.ProjectStatus == ProjectStatus.Active || p.ProjectStatus == ProjectStatus.OnHold)
                        .Where(p => p.ProjectManagerId == employeeId
                        || p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                        .CountAsync();
        }
        // Delete
        public async Task<bool> DeleteAsync(int projectId, int employeeId)
        {
            if (!await ProjectExistsAsync(projectId))
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);
            if (!await IsManagerAsync(projectId, employeeId))
                throw new UnauthorizedAccessException(ErrorShared.Project.OnlyManagerCanDelete);

            var project = await GetByIdAsync(projectId);
            project!.ChangeStatus(ProjectStatus.Cancelled);

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return true;
        }
        // All employees that work on the project
        public async Task<IEnumerable<Employee>?> GetEmployeesAsync(int projectId)
        {
            if (!await ProjectExistsAsync(projectId))
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);

            // return all employees that work on the project
            return await _context.ProjectEmployees
                            .Where(pe => pe.ProjectId == projectId)
                            .Select(pe => pe.Employee)
                            .Where(e => !e.IsDeleted)
                            .ToListAsync();
        }
        // Add employee to project
        public async Task<bool> AddEmployeeToProjectAsync(ProjectEmployee projectEmployee)
        {
            if (!await EmployeeExistsAsync(projectEmployee.EmployeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);
            if (!await ProjectExistsAsync(projectEmployee.ProjectId))
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);

            var empIsAlreadyInProject = await _context.ProjectEmployees
                .AnyAsync(pe => pe.ProjectId == projectEmployee.ProjectId && pe.EmployeeId == projectEmployee.EmployeeId);

            if (empIsAlreadyInProject)
                throw new ArgumentException(ErrorShared.Project.EmployeeAlreadyAssigned);

            if (await IsManagerAsync(projectEmployee.ProjectId, projectEmployee.EmployeeId))
                throw new ArgumentException(ErrorShared.Project.ProjectManagerCannotBeMember);

            if (await IsEmpDeleted(projectEmployee.EmployeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeIsDeleted);

            await _context.ProjectEmployees.AddAsync(projectEmployee);

            var project = await _context.Projects.FindAsync(projectEmployee.ProjectId);
            project?.RecordActivity();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId)
        {
            var projectEmployee = await _context.ProjectEmployees
                .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

            if (projectEmployee is null)
                return false;

            _context.ProjectEmployees.Remove(projectEmployee);

            var project = await _context.Projects.FindAsync(projectId);
            project?.RecordActivity();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmployeeHasActiveTicketsInProjectAsync(int projectId, int employeeId)
        {
            return await _context.Tickets
                .AnyAsync(t =>
                    t.ProjectId == projectId &&
                    t.EmployeeId == employeeId &&
                    t.TicketStatus != TicketStatus.Done &&
                    t.TicketStatus != TicketStatus.Completed &&
                    t.TicketStatus != TicketStatus.Cancelled);
        }

        // Change the status of the project
        public async Task<bool> SetProjectStatusAsync(int projectId, ProjectStatus status)
        {

            if (!await ProjectExistsAsync(projectId))
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);
            if (!System.Enum.IsDefined(status))
                throw new ArgumentException(ErrorShared.Project.InvalidStatus);

            var project = await GetByIdAsync(projectId);
            project!.ChangeStatus(status);

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return true;
        }
        // Get all the projects that the employee works on
        public async Task<IEnumerable<Project>?> GetAllProjectWorkedByEmployeeAsync(int employeeId)
        {
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);
            return await _context.Projects
                        .Where(p => p.ProjectStatus != ProjectStatus.Cancelled)
                        .Include(p => p.ProjectManager)
                        .Include(p => p.ProjectEmployees)
                        .ThenInclude(pe => pe.Employee)
                        .Include(p => p.ProjectTickets)
                        .AsSplitQuery()
                       .Where(p => p.ProjectManagerId == employeeId ||
                        p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                        .ToListAsync();
        }
        // last three projects that the employee added to works on
        public async Task<IEnumerable<String[]>?> GetAllProjectWorkedByEmployeeTopThreeAsync(int employeeId)
        {
            var projects = await GetRecentActiveProjectsAsync(employeeId);

            return projects
                .Select(p => new string[2]
                {
                    p.ProjectName,
                    p.ProjectEmployees
                        .Where(pe => pe.EmployeeId == employeeId)
                        .Select(pe => pe.Role)
                        .FirstOrDefault() ?? ErrorShared.Project.ManagerOrNoRole
                })
                .ToList();
        }

        public async Task<IEnumerable<Project>?> GetDashboardProjectsAsync(int employeeId)
        {
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            return await GetRecentActiveProjectsAsync(employeeId);
        }

        public async Task<IEnumerable<Project>> GetRecentActiveProjectsAsync(int employeeId)
        {
            var projects = await _context.Projects
                .AsNoTracking()
                .Where(p => p.ProjectStatus == ProjectStatus.Active)
                .Where(p => p.ProjectManagerId == employeeId ||
                    p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectEmployees)
                .ThenInclude(pe => pe.Employee)
                .Include(p => p.ProjectTickets)
                .ThenInclude(t => t.TicketHistories)
                .Include(p => p.ProjectTickets)
                .ThenInclude(t => t.AttachmentURL)
                .AsSplitQuery()
                .ToListAsync();

            return projects
                .OrderByDescending(GetLastActivityAt)
                .ThenByDescending(p => p.Id)
                .Take(3)
                .ToList();
        }

        private static DateTime GetLastActivityAt(Project project)
        {
            var lastActivityAt = project.UpdatedAt ?? project.CreatedAt;

            foreach (var ticket in project.ProjectTickets)
            {
                if (ticket.CreatedAt > lastActivityAt)
                    lastActivityAt = ticket.CreatedAt;

                if (ticket.UpdatedAt is DateTime ticketUpdatedAt && ticketUpdatedAt > lastActivityAt)
                    lastActivityAt = ticketUpdatedAt;

                foreach (var history in ticket.TicketHistories)
                {
                    if (history.ModifiedAt > lastActivityAt)
                        lastActivityAt = history.ModifiedAt;
                }

                foreach (var attachment in ticket.AttachmentURL)
                {
                    if (attachment.CreatedAt > lastActivityAt)
                        lastActivityAt = attachment.CreatedAt;
                }
            }

            return lastActivityAt;
        }

        // Filter the projects that the employee works on by status
        public async Task<IEnumerable<Project>?> GetAllProjectWorkedByEmployeeWithFilterAsync(int employeeId,
            ProjectStatus FilterByStatus)
        {
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            return await _context.Projects
                         .Where(p => p.ProjectStatus == FilterByStatus)
                         .Include(p => p.ProjectManager)
                         .Include(p => p.ProjectEmployees)
                         .ThenInclude(pe => pe.Employee)
                         .Include(p => p.ProjectTickets)
                         .AsSplitQuery()
                         .Where(p => p.ProjectManagerId == employeeId
                         || p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                         .ToListAsync();
        }


        public async Task<bool> IsManagerAsync(int projectId, int employeeId)
            => await _context.Projects.Where(p => p.Id == projectId).AnyAsync(e => e.ProjectManagerId == employeeId);
        public async Task<bool> EmployeeExistsAsync(int employeeId)
            => await _context.Employees.AnyAsync(e => e.Id == employeeId && !e.IsDeleted);
        public async Task<bool> ProjectExistsAsync(int projectId)
            => await _context.Projects.AnyAsync(p => p.Id == projectId && p.ProjectStatus != ProjectStatus.Cancelled);
        public async Task<bool> TicketExistsAsync(int projectId)
            => await _context.Tickets.AnyAsync(t => t.ProjectId == projectId);
        public async Task<bool> IsEmpDeleted(int employeeId)
        {
            return await _context.Employees
                 .Where(e => e.Id == employeeId && e.IsDeleted == true)
                 .Select(e => e.IsDeleted)
                 .FirstOrDefaultAsync();
        }
    }
}
