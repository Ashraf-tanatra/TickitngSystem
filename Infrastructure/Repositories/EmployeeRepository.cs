using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees.ToList();
        }

        public Employee? GetById(int id)
        {
            return _context.Employees.FirstOrDefault(e => e.Id == id);
        }

        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();
        }

        public void Delete(Employee employee)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();
        }

        public bool ExistsByEmail(string email)
        {
            return _context.Employees
                .Any(e => e.Account.Email == email);
        }

        public bool ExistsByPhone(string phone)
        {
            return _context.Employees
                .Any(e => e.Phone == phone);
        }

        public bool ExistsByPhoneExcept(string phone, int employeeId)
        {
            return _context.Employees
                .Any(e => e.Phone == phone && e.Id != employeeId);
        }
        public IEnumerable<Project> GetProjects(int employeeId)
        {
            return _context.Projects
                .Where(p => p.ProjectEmployees
                    .Any(pe => pe.EmployeeId == employeeId))
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.ProjectTickets)
                .ToList();
        }
    }
}