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
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .FirstOrDefaultAsync(t => t.TicketId == id);
        }
        public async Task<bool> Create(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<bool> Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<bool> Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<int> GetTicketTotalCountForAnEmployee(int employeeId)
        {
            return await _context.Tickets
                .CountAsync(t => t.EmployeeId == employeeId
                && t.TicketStatus != TicketStatus.Done
                && t.TicketStatus != TicketStatus.Cancelled);
        }
        public async Task<int> GetTicketCompletedCountForAnEmployee(int employeeId)
        {
            return await _context.Tickets.CountAsync(t => t.EmployeeId == employeeId && t.TicketStatus == TicketStatus.Completed);
        }
        public async Task<int> GetTicketInProgressCountForAnEmployee(int employeeId)
        {
            return await _context.Tickets.CountAsync(t => t.EmployeeId == employeeId && t.TicketStatus == TicketStatus.InProgress);
        }
        public async Task<bool> AddAttachmentToTicket(int ticketId, string filePath)
        {
            var attachment = new TicketAttachments
            {
                URL = filePath,
                TicketId = ticketId
            };
            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<bool> ChangeTicketStatus(int ticketId, TicketStatus status)
        {
            await _context.Tickets.Where(t => t.TicketId == ticketId)
                 .ExecuteUpdateAsync(s => s.SetProperty(t => t.TicketStatus, status));
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<IEnumerable<Ticket>?> GetAllTicketsForAProject(int projectId)
        {
            return await _context.Tickets
                .Where(t => t.ProjectId == projectId)
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .ToListAsync();
        }
        public async Task<IEnumerable<Ticket>?> GetAllTicketsForAnEmployee(int employeeId)
        {
            return await _context.Tickets
                .Where(t => t.EmployeeId == employeeId
                && t.TicketStatus != TicketStatus.Done
                && t.TicketStatus != TicketStatus.Cancelled)
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .ToListAsync();
        }
        public async Task<bool> ChangeTicketPriority(int ticketId, TicketPriority priority)
        {
            await _context.Tickets.Where(t => t.TicketId == ticketId)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.Priority, priority));
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }


        public async Task<bool> TicketExists(int ticketId)
        {
            return await _context.Tickets.AnyAsync(t => t.TicketId == ticketId);
        }
        public async Task<bool> EmployeeExists(int employeeId)
        {
            return await _context.Employees.AnyAsync(e => e.Id == employeeId);
        }
        public async Task<bool> ProjectExists(int projectId)
        {
            return await _context.Projects.AnyAsync(p => p.Id == projectId);
        }
        public async Task<bool> IsManager(int employeeId, int projectId)
        {
            return await _context.Projects
                .Where(p => p.Id == projectId)
                .AnyAsync(x => x.ProjectManagerId == employeeId);
        }
    }
}