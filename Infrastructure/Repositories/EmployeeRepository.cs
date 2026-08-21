using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }


        // GET ALL
        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees
                .ToList();
        }


        // GET BY ID
        public Employee? GetById(int id)
        {
            return _context.Employees
                .FirstOrDefault(e => e.Id == id);
        }


        // ADD
        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }


        // UPDATE
        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();
        }


        // CHECK PHONE
        public bool ExistsByPhone(string phone)
        {
            return _context.Employees
                .Any(e =>
                    e.Phone == phone &&
                    !e.IsDeleted);
        }


        // CHECK PHONE EXCEPT CURRENT EMPLOYEE
        public bool ExistsByPhoneExcept(string phone,int employeeId)
        {
            return _context.Employees
                .Any(e =>
                    e.Phone == phone &&
                    e.Id != employeeId &&
                    !e.IsDeleted);
        }
        



        // GET EMPLOYEE PROJECTS
        public IEnumerable<Project> GetProjects(
            int employeeId)
        {
            return _context.Projects
                .Where(p => p.ProjectEmployees
                    .Any(pe =>
                        pe.EmployeeId == employeeId))

                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.Employee)

                .Include(p => p.ProjectTickets)

                .ToList();
        }
    }
}