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
                .Where(t => t.ProjectId == projectId
                    && t.TicketStatus != TicketStatus.Cancelled)
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
                .Include(t => t.AttachmentURL)
                .FirstOrDefaultAsync(t => t.TicketId == id);
        }

        public async Task<IEnumerable<TicketHistory>> GetTicketHistoryAsync(int ticketId)
        {
            return await _context.TicketHistories
                .Where(h => h.TicketId == ticketId)
                .Include(h => h.ActionByEmployee)
                .Include(h => h.FromEmployee)
                .Include(h => h.ToEmployee)
                .OrderByDescending(h => h.ModifiedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketAttachments>> GetTicketAttachmentsAsync(int ticketId)
        {
            return await _context.Attachments
                .Where(attachment => attachment.TicketId == ticketId)
                .OrderByDescending(attachment => attachment.CreatedAt)
                .ToListAsync();
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

        public async Task UpdateWithHistoryAsync(Ticket ticket, TicketHistory history)
        {
            _context.Tickets.Update(ticket);
            await _context.TicketHistories.AddAsync(history);
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

        public async Task<int> GetTicketNeedReviewCountForAnEmployeeAsync(int employeeId)
        {
            return await _context.Tickets.CountAsync(t =>
                t.EmployeeId == employeeId &&
                (t.TicketStatus == TicketStatus.NeedReview ||
                 t.TicketStatus == TicketStatus.InReview));
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

        public async Task AddAttachmentToTicketAsync(
            int ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes)
        {
            var attachment = TicketAttachments.Create(
                ticketId,
                url,
                originalFileName,
                storedFileName,
                contentType,
                sizeInBytes);

            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TicketExistsAsync(int ticketId)
        {
            return await _context.Tickets.AnyAsync(t => t.TicketId == ticketId);
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.Id == employeeId && !e.IsDeleted);
        }

        public async Task<bool> ProjectExistsAsync(int projectId)
        {
            return await _context.Projects
                .AnyAsync(p => p.Id == projectId && p.ProjectStatus != ProjectStatus.Cancelled);
        }

        public async Task<bool> IsManagerAsync(int employeeId, int projectId)
        {
            return await _context.Projects
                .Where(p => p.Id == projectId && p.ProjectStatus != ProjectStatus.Cancelled)
                .AnyAsync(x => x.ProjectManagerId == employeeId);
        }

        public async Task<bool> IsEmployeeAssignedToProjectAsync(int employeeId, int projectId)
        {
            return await _context.Projects
                .AnyAsync(project =>
                    project.Id == projectId &&
                    project.ProjectStatus != ProjectStatus.Cancelled &&
                    (project.ProjectManagerId == employeeId ||
                     project.ProjectEmployees.Any(projectEmployee =>
                         projectEmployee.EmployeeId == employeeId &&
                         !projectEmployee.Employee.IsDeleted)));
        }
    }
}
