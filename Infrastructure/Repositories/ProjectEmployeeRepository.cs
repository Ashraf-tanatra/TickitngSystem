using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectEmployeeRepository : IProjectEmployeeRepository
    {
        private readonly AppDbContext _context;

        public ProjectEmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectEmployee>> GetAllAsync()
        {
            return await _context.ProjectEmployees
                .Include(pe => pe.Project)
                .Include(pe => pe.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectEmployee>> GetByProjectIdAsync(int projectId)
        {
            return await _context.ProjectEmployees
                .Where(pe => pe.ProjectId == projectId)
                .Include(pe => pe.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectEmployee>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.ProjectEmployees
                .Where(pe => pe.EmployeeId == employeeId)
                .Include(pe => pe.Project)
                .ToListAsync();
        }

        public async Task<ProjectEmployee?> GetAsync(int projectId, int employeeId)
        {
            return await _context.ProjectEmployees
                .Include(pe => pe.Project)
                .Include(pe => pe.Employee)
                .FirstOrDefaultAsync(pe =>
                    pe.ProjectId == projectId &&
                    pe.EmployeeId == employeeId);
        }

        public async Task<bool> ExistsAsync(int projectId, int employeeId)
        {
            return await _context.ProjectEmployees
                .AnyAsync(pe =>
                    pe.ProjectId == projectId &&
                    pe.EmployeeId == employeeId);
        }

        public async Task AddAsync(ProjectEmployee projectEmployee)
        {
            await _context.ProjectEmployees.AddAsync(projectEmployee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProjectEmployee projectEmployee)
        {
            _context.ProjectEmployees.Remove(projectEmployee);
            await _context.SaveChangesAsync();
        }
    }
}
