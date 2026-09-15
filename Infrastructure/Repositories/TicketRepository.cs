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
        public async Task<IEnumerable<Ticket>> GetAllTicketsForAProjectAsync(Guid projectId)
        {
            return await _context.Tickets
                .Where(t => t.ProjectId == projectId
                    && t.TicketStatus != TicketStatus.Cancelled)
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsForAnEmployeeAsync(Guid employeeId)
        {
            return await _context.Tickets
                .Where(t => t.EmployeeId == employeeId
                && t.TicketStatus != TicketStatus.Done
                && t.TicketStatus != TicketStatus.Cancelled)
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _context.Tickets
                .Include(e => e.Employee)
                .Include(p => p.Project)
                .Include(t => t.AttachmentURL)
                .FirstOrDefaultAsync(t => t.TicketId == id);
        }

        public async Task<IEnumerable<TicketHistory>> GetTicketHistoryAsync(Guid ticketId)
        {
            return await _context.TicketHistories
                .Where(h => h.TicketId == ticketId)
                .Include(h => h.ActionByEmployee)
                .Include(h => h.FromEmployee)
                .Include(h => h.ToEmployee)
                .OrderByDescending(h => h.ModifiedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketAttachments>> GetTicketAttachmentsAsync(Guid ticketId)
        {
            return await _context.Attachments
                .Where(attachment => attachment.TicketId == ticketId)
                .OrderByDescending(attachment => attachment.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);

            var project = await _context.Projects.FindAsync(ticket.ProjectId);
            project?.RecordActivity();

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);

            var project = await _context.Projects.FindAsync(ticket.ProjectId);
            project?.RecordActivity();

            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithHistoryAsync(Ticket ticket, TicketHistory history)
        {
            _context.Tickets.Update(ticket);
            await _context.TicketHistories.AddAsync(history);

            var project = await _context.Projects.FindAsync(ticket.ProjectId);
            project?.RecordActivity();

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTicketTotalCountForAnEmployeeAsync(Guid employeeId)
        {
            return await _context.Tickets
                .CountAsync(t => t.EmployeeId == employeeId
                && t.TicketStatus != TicketStatus.Done
                && t.TicketStatus != TicketStatus.Cancelled);
        }

        public async Task<int> GetTicketInProgressCountForAnEmployeeAsync(Guid employeeId)
        {
            return await _context.Tickets.CountAsync(t => t.EmployeeId == employeeId && t.TicketStatus == TicketStatus.InProgress);
        }

        public async Task<int> GetTicketCompletedCountForAnEmployeeAsync(Guid employeeId)
        {
            return await _context.Tickets.CountAsync(t => t.EmployeeId == employeeId && t.TicketStatus == TicketStatus.Completed);
        }

        public async Task<int> GetTicketNeedReviewCountForAnEmployeeAsync(Guid employeeId)
        {
            return await _context.Tickets.CountAsync(t =>
                t.EmployeeId == employeeId &&
                (t.TicketStatus == TicketStatus.NeedReview ||
                 t.TicketStatus == TicketStatus.InReview));
        }

        public async Task ChangeTicketStatusAsync(Guid ticketId, TicketStatus status)
        {
            var ticket = await GetByIdAsync(ticketId);

            if (ticket is null)
            {
                return;
            }

            ticket.ChangeStatus(status);
            _context.Tickets.Update(ticket);

            ticket.Project.RecordActivity();

            await _context.SaveChangesAsync();
        }

        public async Task ChangeTicketPriorityAsync(Guid ticketId, TicketPriority priority)
        {
            var ticket = await GetByIdAsync(ticketId);

            if (ticket is null)
            {
                return;
            }

            ticket.ChangePriority(priority);
            _context.Tickets.Update(ticket);

            ticket.Project.RecordActivity();

            await _context.SaveChangesAsync();
        }

        public async Task AddAttachmentToTicketAsync(
            Guid ticketId,
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

            var projectId = await _context.Tickets
                .Where(t => t.TicketId == ticketId)
                .Select(t => t.ProjectId)
                .SingleAsync();
            var project = await _context.Projects.FindAsync(projectId);
            project?.RecordActivity();

            await _context.SaveChangesAsync();
        }

        public async Task<bool> TicketExistsAsync(Guid ticketId)
        {
            return await _context.Tickets.AnyAsync(t => t.TicketId == ticketId);
        }

        public async Task<bool> EmployeeExistsAsync(Guid employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.Id == employeeId && !e.IsDeleted);
        }

        public async Task<bool> ProjectExistsAsync(Guid projectId)
        {
            return await _context.Projects
                .AnyAsync(p => p.Id == projectId && p.ProjectStatus != ProjectStatus.Cancelled);
        }

        public async Task<bool> IsManagerAsync(Guid employeeId, Guid projectId)
        {
            return await _context.Projects
                .Where(p => p.Id == projectId && p.ProjectStatus != ProjectStatus.Cancelled)
                .AnyAsync(x => x.ProjectManagerId == employeeId);
        }

        public async Task<bool> IsEmployeeAssignedToProjectAsync(Guid employeeId, Guid projectId)
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
