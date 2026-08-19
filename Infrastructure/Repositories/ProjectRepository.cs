using Domain.Entities;
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

        public IEnumerable<Project> GetAll()
        {
            return _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectTickets)
                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.Employee)
                .ToList();
        }

        public Project? GetById(int id)
        {
            return _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectTickets)
                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.Employee)
                .FirstOrDefault(p => p.Id == id);
        }

        public void Add(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        public void Update(Project project)
        {
            _context.Projects.Update(project);
            _context.SaveChanges();
        }

        public void Delete(Project project)
        {
            _context.Projects.Remove(project);
            _context.SaveChanges();
        }

        public bool EmployeeExists(int employeeId)
        {
            return _context.Employees
                .Any(e => e.Id == employeeId);
        }

        //public bool IsManager(int employeeId)
        //{
        //    return _context.Employees
        //        .Any(e => e.Id == employeeId &&
        //                 e.Role == EmployeeRole.Manager);
        //}

        public IEnumerable<Employee> GetEmployees(int projectId)
        {
            return _context.ProjectEmployees
                .Where(pe => pe.ProjectId == projectId)
                .Select(pe => pe.Employee)
                .Where(e => !e.IsDeleted)
                .ToList();
        }

        public IEnumerable<Ticket> GetTickets(int projectId)
        {
            return _context.Tickets
                .Where(t => t.ProjectId == projectId)
                .ToList();
        }
    }
}