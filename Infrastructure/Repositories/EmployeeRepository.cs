using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;
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

        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.Account)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // =========================================================
        // ADD
        // =========================================================

        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // CHECK PHONE
        // =========================================================

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await _context.Employees
                .AnyAsync(e =>
                    e.Phone == phone &&
                    !e.IsDeleted);
        }

        // =========================================================
        // CHECK PHONE EXCEPT CURRENT EMPLOYEE
        // =========================================================

        public async Task<bool> ExistsByPhoneExceptAsync(
            string phone,
            Guid employeeId)
        {
            return await _context.Employees
                .AnyAsync(e =>
                    e.Phone == phone &&
                    e.Id != employeeId &&
                    !e.IsDeleted);
        }

        // =========================================================
        // GET PROJECTS
        // =========================================================

        public async Task<IEnumerable<Project>> GetProjectsAsync(
            Guid employeeId)
        {
            return await _context.Projects
                .Where(p => p.ProjectStatus != ProjectStatus.Cancelled)
                .Where(p => p.ProjectManagerId == employeeId ||
                    p.ProjectEmployees.Any(pe =>
                        pe.EmployeeId == employeeId &&
                        !pe.Employee.IsDeleted))
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectEmployees)
                .ThenInclude(pe => pe.Employee)
                .Include(p => p.ProjectTickets)
                .AsSplitQuery()
                .ToListAsync();
        }
        // =========================================================
        // GET ACTIVE PROJECTS
        // =========================================================
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync(Guid employeeId)
        {
            return await _context.Projects
                .Where(p =>
                    p.ProjectStatus == ProjectStatus.Active &&
                    p.ProjectManagerId == employeeId)
                .Include(p => p.ProjectEmployees)
                .Include(p => p.ProjectTickets)
                .ToListAsync();
        }

        // =========================================================
        // GET Employee Tickets
        // =========================================================

        public async Task<IEnumerable<Ticket>> GetEmployeeTicketsAsync(Guid employeeId)
        {
            return await _context.Tickets.Where(t => t.EmployeeId == employeeId).ToListAsync();

        }

    }
}
