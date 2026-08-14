using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
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

        public IEnumerable<ProjectEmployee> GetAll()
        {
            return _context.ProjectEmployees.ToList();
        }

        public IEnumerable<ProjectEmployee> GetByProjectId(int projectId)
        {
            return _context.ProjectEmployees
                           .Where(pe => pe.ProjectId == projectId)
                           .ToList();
        }

        public IEnumerable<ProjectEmployee> GetByEmployeeId(int employeeId)
        {
            return _context.ProjectEmployees
                           .Where(pe => pe.EmployeeId == employeeId)
                           .ToList();
        }

        public ProjectEmployee? Get(int projectId, int employeeId)
        {
            return _context.ProjectEmployees
                           .FirstOrDefault(pe =>
                               pe.ProjectId == projectId &&
                               pe.EmployeeId == employeeId);
        }

        public bool Exists(int projectId, int employeeId)
        {
            return _context.ProjectEmployees
                           .Any(pe =>
                               pe.ProjectId == projectId &&
                               pe.EmployeeId == employeeId);
        }

        public void Add(ProjectEmployee projectEmployee)
        {
            _context.ProjectEmployees.Add(projectEmployee);
            _context.SaveChanges();
        }

        public void Delete(ProjectEmployee projectEmployee)
        {
            _context.ProjectEmployees.Remove(projectEmployee);
            _context.SaveChanges();
        }
    }
}