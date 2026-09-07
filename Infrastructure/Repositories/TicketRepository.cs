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
        public async Task<IEnumerable<Ticket>> GetAllTicketsForAProjectAsync(int projectId)
        {
            return await _context.Tickets
                .Where(t => t.ProjectId == projectId)
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsForAnEmployeeAsync(int employeeId)
        {
            return await _context.Tickets
                .Where(t => t.EmployeeId == employeeId
                && t.TicketStatus != TicketStatus.Done
                && t.TicketStatus != TicketStatus.Cancelled)
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .FirstOrDefaultAsync(t => t.TicketId == id);
        }

        public async Task CreateAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTicketTotalCountForAnEmployeeAsync(int employeeId)
        {
            return await _context.Tickets
                .CountAsync(t => t.EmployeeId == employeeId
                && t.TicketStatus != TicketStatus.Done
                && t.TicketStatus != TicketStatus.Cancelled);
        }

        public async Task<int> GetTicketInProgressCountForAnEmployeeAsync(int employeeId)
        {
            return await _context.Tickets.CountAsync(t => t.EmployeeId == employeeId && t.TicketStatus == TicketStatus.InProgress);
        }

        public async Task<int> GetTicketCompletedCountForAnEmployeeAsync(int employeeId)
        {
            return await _context.Tickets.CountAsync(t => t.EmployeeId == employeeId && t.TicketStatus == TicketStatus.Completed);
        }

        public async Task ChangeTicketStatusAsync(int ticketId, TicketStatus status)
        {
            var ticket = await GetByIdAsync(ticketId);

            if (ticket is null)
            {
                return;
            }

            ticket.ChangeStatus(status);
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeTicketPriorityAsync(int ticketId, TicketPriority priority)
        {
            var ticket = await GetByIdAsync(ticketId);

            if (ticket is null)
            {
                return;
            }

            ticket.ChangePriority(priority);
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task AddAttachmentToTicketAsync(int ticketId, string filePath)
        {
            var attachment = TicketAttachments.Create(ticketId, filePath);

            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TicketExistsAsync(int ticketId)
        {
            return await _context.Tickets.AnyAsync(t => t.TicketId == ticketId);
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees.AnyAsync(e => e.Id == employeeId);
        }

        public async Task<bool> ProjectExistsAsync(int projectId)
        {
            return await _context.Projects.AnyAsync(p => p.Id == projectId);
        }

        public async Task<bool> IsManagerAsync(int employeeId, int projectId)
        {
            return await _context.Projects
                .Where(p => p.Id == projectId)
                .AnyAsync(x => x.ProjectManagerId == employeeId);
        }
    }
}
