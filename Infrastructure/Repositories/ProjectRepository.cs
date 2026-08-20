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

        // add order by Get all the projects that the employee works on
        public IEnumerable<Project> GetAllProjectWorkedByEmployee(int employeeId)
        {
            return _context.Projects
                    .Where(p => p.ProjectStatus == ProjectStatus.Active)
                    .Include(p => p.ProjectEmployees)
                    .Where(p => p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                    .ToList();
        }
        // add order by last three projects that the employee added to works on
        public IEnumerable<Project> GetAllProjectWorkedByEmployeeTopThree(int employeeId)
        {
            return _context.Projects
                   .Where(p => p.ProjectStatus == ProjectStatus.Active)
                   .Include(p => p.ProjectEmployees)
                   .Where(p => p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                   .Take(3)
                   .ToList();
            //OrderBy(d=>d.EndTime) //to do after edit the database
        }
        //Number of projects that the employee works on
        public int GetProjectCount(int employeeId)
        {
            return _context.Projects
                   .Where(p => p.ProjectStatus == ProjectStatus.Active)
                   .Include(p => p.ProjectEmployees)
                   .Where(p => p.ProjectEmployees.Any(pe => pe.EmployeeId == employeeId))
                   .Count();
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
        // Delete // ? may not Delete the project just set it as cancelled
        public void Delete(Project project)
        {
            _context.Projects.Remove(project);
            _context.SaveChanges();
        }

        public void SetProjectAsActive(int projectId)
        {
            _context.Projects
                .Where(p => p.Id == projectId)
                .ExecuteUpdate(setter => setter.SetProperty(p => p.ProjectStatus, ProjectStatus.Active));
        }
        public void SetProjectAsCancelled(int projectId)
        {
            _context.Projects
              .Where(p => p.Id == projectId)
              .ExecuteUpdate(setter => setter.SetProperty(p => p.ProjectStatus, ProjectStatus.Cancelled));
        }
        public void SetProjectAsCompleted(int projectId)
        {
            _context.Projects
              .Where(p => p.Id == projectId)
              .ExecuteUpdate(setter => setter.SetProperty(p => p.ProjectStatus, ProjectStatus.Completed));
        }

        // ?
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

        //public IEnumerable<Employee> GetEmployees(int projectId)
        //{
        //    return _context.ProjectEmployees
        //        .Where(pe => pe.ProjectId == projectId)
        //        .Select(pe => pe.Employee)
        //        .Where(e => !e.IsDeleted)
        //        .ToList();
        //}

        //public IEnumerable<Ticket> GetTickets(int projectId)
        //{
        //    return _context.Tickets
        //        .Where(t => t.ProjectId == projectId)
        //        .ToList();
        //}

        //public IEnumerable<Project> GetAll()
        //{
        //    return _context.Projects
        //        .Include(p => p.ProjectManager)
        //        .Include(p => p.ProjectTickets)
        //        .Include(p => p.ProjectEmployees)
        //            .ThenInclude(pe => pe.Employee)
        //        .ToList();
        //}
    }
}