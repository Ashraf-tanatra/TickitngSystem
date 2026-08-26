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
            if (!await ProjectExistsAsync(id))
                throw new NullReferenceException("The project does not exists!!!");

            return await _context.Projects
                .Include(p => p.ProjectEmployees)
                .ThenInclude(pe => pe.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        // Update
        public async Task<bool> UpdateAsync(Project project)
        {
            if (!await ProjectExistsAsync(project.Id))
                throw new ArgumentException("Project does not exist.");

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        // Create
        public async Task<bool> CreateAsync(Project project)
        {
            if (!await EmployeeExistsAsync(project.ProjectManagerId))
                throw new ArgumentException("Employee does not exist.");
            if (await ProjectExistsAsync(project.Id))
                throw new ArgumentException("Project already exists.");

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        //Number of projects that the employee works on
        public async Task<int> GetProjectCountAsync(int employeeId)
        {
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException("Employee does not exist.");

            return await _context.Projects
                        .Where(p => p.ProjectStatus == ProjectStatus.Active || p.ProjectStatus == ProjectStatus.OnHold)
                        .Where(p => p.ProjectManagerId == employeeId
                        || (p.ProjectEmployees != null && p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId)))
                        .CountAsync();
        }
        // Delete
        public async Task<bool> DeleteAsync(int projectId, int employeeId)
        {
            if (!await ProjectExistsAsync(projectId))
                throw new ArgumentException("Project does not exist.");
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException("Employee does not exist.");
            if (!await IsManagerAsync(projectId, employeeId))
                throw new UnauthorizedAccessException("Project cannot delete by you.");

            // If the project has tickets, make it cancelled and do not delete it from the database
            if (await TicketExistsAsync(projectId))
            {
                await _context.Projects.Where(p => p.Id == projectId)
                     .ExecuteUpdateAsync(setter => setter.SetProperty(p => p.ProjectStatus, ProjectStatus.Cancelled));
                return await Task.FromResult(true);
            }
            else
            {
                var project = await GetByIdAsync(projectId);
                // If the project has no tickets, delete it from the database
                _context.Projects.Remove(project!);
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
        }
        // All employees that work on the project
        public async Task<IEnumerable<Employee>?> GetEmployeesAsync(int projectId)
        {
            if (!await ProjectExistsAsync(projectId))
                throw new ArgumentException($"Project does not exist.");

            // return all employees that work on the project
            return await _context.ProjectEmployees
                            .Where(pe => pe.ProjectId == projectId)
                            .Select(pe => pe.Employee)
                            //.Where(e => !e.IsDeleted)
                            .ToListAsync();
        }
        // Add employee to project
        public async Task<bool> AddEmployeeToProjectAsync(ProjectEmployee projectEmployee)
        {
            if (!await EmployeeExistsAsync(projectEmployee.EmployeeId))
                throw new ArgumentException($"Employee does not exist.");
            if (!await ProjectExistsAsync(projectEmployee.ProjectId))
                throw new ArgumentException($"Project does not exist.");

            var empIsAlreadyInProject = await _context.ProjectEmployees
                .AnyAsync(pe => pe.ProjectId == projectEmployee.ProjectId && pe.EmployeeId == projectEmployee.EmployeeId);

            if (empIsAlreadyInProject)
                throw new ArgumentException($"Employee is already in the project.");

            if (await IsEmpDeleted(projectEmployee.EmployeeId))
                throw new ArgumentException($"Employee is deleted.");

            await _context.ProjectEmployees.AddAsync(projectEmployee);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        // Change the status of the project
        public async Task<bool> SetProjectStatusAsync(int projectId, ProjectStatus status)
        {

            if (!await ProjectExistsAsync(projectId))
                throw new ArgumentException($"Project does not exist.");
            if ((int)status < 0 || (int)status > 4)
                throw new ArgumentException("Invalid project status.");

            await _context.Projects
                      .Where(p => p.Id == projectId)
                      .ExecuteUpdateAsync(setter => setter.SetProperty(p => p.ProjectStatus, status));
            return await Task.FromResult(true);
        }
        // Get all the projects that the employee works on
        public async Task<IEnumerable<Project>?> GetAllProjectWorkedByEmployeeAsync(int employeeId)
        {
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException($"Employee does not exist.");
            //problem if the manager is exits 
            //if (await _context.ProjectEmployees.FirstOrDefaultAsync(pe => pe.EmployeeId == employeeId) == null)
            //    throw new ArgumentException("Employee does not work on any project.");

            return await _context.Projects
                        .Where(p => p.ProjectStatus != ProjectStatus.Cancelled)
                        .Include(p => p.ProjectEmployees)
                       .Where(p => p.ProjectManagerId == employeeId ||
                        (p.ProjectEmployees != null && p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId)))
                        .ToListAsync();
        }
        // last three projects that the employee added to works on
        public async Task<IEnumerable<String[]>?> GetAllProjectWorkedByEmployeeTopThreeAsync(int employeeId)
        {
            return await _context.Projects
                   .Where(p => p.ProjectStatus == ProjectStatus.Active || p.ProjectStatus == ProjectStatus.OnHold)
                   .Include(p => p.ProjectEmployees)
                   .Where(p => p.ProjectManagerId == employeeId ||
                   (p.ProjectEmployees != null && p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId)))
                   .OrderByDescending(d => d.StartedAt)
                   .Take(3)
                   .Select(p => new string[2]
                  {
                        p.ProjectName ?? string.Empty,
                        p.ProjectEmployees == null
                            ? "Manager/No Role"
                            : (p.ProjectEmployees
                                .Where(pe => pe.EmployeeId == employeeId)
                                .Select(pe => pe.Role)
                                .FirstOrDefault() ?? "Manager/No Role")
                    })
                   .ToListAsync();
        }
        // Filter the projects that the employee works on by status
        public async Task<IEnumerable<Project>?> GetAllProjectWorkedByEmployeeWithFilterAsync(int employeeId,
            ProjectStatus FilterByStatus)
        {
            if (!await EmployeeExistsAsync(employeeId))
                throw new ArgumentException($"Employee does not exist.");

            return await _context.Projects
                         .Where(p => p.ProjectStatus == FilterByStatus)
                         .Include(p => p.ProjectEmployees)
                         .Where(p => p.ProjectManagerId == employeeId
                         || p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                         .ToListAsync();
        }

        IEnumerable<Ticket> IProjectRepository.GetTicketsAsync(int projectId)
        {
            var tickets = _context.Tickets.Where(t => t.ProjectId == projectId).ToList();
            return tickets;
        }


        public async Task<bool> IsManagerAsync(int projectId, int employeeId)
            => await _context.Projects.Where(p => p.Id == projectId).AnyAsync(e => e.ProjectManagerId == employeeId);
        public async Task<bool> EmployeeExistsAsync(int employeeId)
            => await _context.Employees.AnyAsync(e => e.Id == employeeId);
        public async Task<bool> ProjectExistsAsync(int projectId)
            => await _context.Projects.AnyAsync(p => p.Id == projectId);
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