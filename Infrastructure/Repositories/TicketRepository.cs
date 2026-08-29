using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Ticket?> GetById(int id)
        {
            return await _context.Tickets
                  .Include(t => t.Project)
                  .AsNoTracking()
                  .FirstOrDefaultAsync(t => t.TicketId == id);
        }
        public async Task<bool> Delete(int ticketId)
        {
            var rowEffected = await _context.Tickets
                .Where(t => t.TicketId == ticketId)
                .ExecuteDeleteAsync();

            return rowEffected > 0;
        }
        public async Task<bool> Create(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            _context.Tickets.Add(ticket);
            return await _context.SaveChangesAsync() > 0;

        }
        public async Task<bool> Update(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            var ticketExist = await TicketExists(ticket.TicketId);
            var empExist = await EmployeeExists(ticket.EmployeeId);
            var isManager = await IsManager(ticket.TicketCreatedById, ticket.ProjectId);

            if (!ticketExist || !empExist || !isManager)
                return false;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
        // maybe need some optimization
        public Task<int> GetTicketTotalCountForAnEmployee(int employeeId)
        {
            return _context.Tickets
                .CountAsync(t => t.EmployeeId == employeeId
                && t.TicketStatus != TicketStatus.Done
                && t.TicketStatus != TicketStatus.Cancelled);
        }
        public Task<int> GetTicketCompletedCountForAnEmployee(int employeeId)
        {
            return _context.Tickets.CountAsync(t => t.EmployeeId == employeeId && t.TicketStatus == TicketStatus.Completed);
        }
        public Task<int> GetTicketInProgressCountForAnEmployee(int employeeId)
        {
            return _context.Tickets.CountAsync(t => t.EmployeeId == employeeId && t.TicketStatus == TicketStatus.InProgress);
        }
        public async Task<bool> AddAttachmentToTicket(int ticketId, string filePath)
        {
            var attachment = new TicketAttachments
            {
                URL = filePath,
                TicketId = ticketId
            };
            _context.Attachments.Add(attachment);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> ChangeTicketStatus(int ticketId, TicketStatus status)// need validate that the emp assign to is the who is change it
        {
            var rowEffected = await _context.Tickets
                                    .Where(t => t.TicketId == ticketId)
                                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.TicketStatus, status));

            return rowEffected > 0;
        }
        public async Task<IEnumerable<Ticket>> GetAllTicketsForAProject(int projectId)
        {
            return await _context.Tickets
                .Where(t => t.ProjectId == projectId)
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Ticket>> GetAllTicketsForAnEmployee(int employeeId)
        {
            return await _context.Tickets
                .Where(t => t.EmployeeId == employeeId
                && t.TicketStatus != TicketStatus.Done
                && t.TicketStatus != TicketStatus.Cancelled)
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<bool> ChangeTicketPriority(int ticketId, TicketPriority priority) // need validate that the emp assign to is the who is change it
        {
            var rowEffected = await _context.Tickets
                                    .Where(t => t.TicketId == ticketId)
                                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.Priority, priority));

            return rowEffected > 0;
        }


        public async Task<string?> GetEmpName(int ticketId)
        {
            return await _context.Tickets
                  .Where(t => t.TicketId == ticketId && t.Employee != null)
                  .Select(t => t.Employee!.FName + " " + t.Employee.LName)
                  .FirstOrDefaultAsync();
        }
        public Task<bool> IsManager(int employeeId, int projectId)
        {
            return _context.Projects
                .Where(p => p.Id == projectId)
                .AnyAsync(x => x.ProjectManagerId == employeeId);
        }
        public Task<bool> ProjectExists(int projectId) => _context.Projects.AnyAsync(p => p.Id == projectId);
        public Task<bool> TicketExists(int ticketId) => _context.Tickets.AnyAsync(t => t.TicketId == ticketId);
        public Task<bool> EmployeeExists(int employeeId) => _context.Employees.AnyAsync(e => e.Id == employeeId);
    }
}