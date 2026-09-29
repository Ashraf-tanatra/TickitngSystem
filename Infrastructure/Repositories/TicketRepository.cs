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

        public async Task<IEnumerable<Ticket>> GetRecentTicketsWithActivityAsync(Guid employeeId)
        {
            var tickets = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.EmployeeId == employeeId
                    && t.TicketStatus != TicketStatus.Done
                    && t.TicketStatus != TicketStatus.Cancelled)
                .Include(t => t.Employee)
                .Include(t => t.Project)
                .Include(t => t.TicketHistories)
                .Include(t => t.AttachmentURL)
                .AsSplitQuery()
                .ToListAsync();

            return tickets
                .OrderByDescending(GetLastActivityAt)
                .ThenByDescending(t => t.TicketId)
                .Take(3)
                .ToList();
        }

        private static DateTime GetLastActivityAt(Ticket ticket)
        {
            var lastActivityAt = ticket.UpdatedAt ?? ticket.CreatedAt;

            foreach (var history in ticket.TicketHistories)
            {
                if (history.ModifiedAt > lastActivityAt)
                    lastActivityAt = history.ModifiedAt;
            }

            foreach (var attachment in ticket.AttachmentURL)
            {
                if (attachment.CreatedAt > lastActivityAt)
                    lastActivityAt = attachment.CreatedAt;
            }

            return lastActivityAt;
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

        public async Task<TicketAttachments?> GetAttachmentByStoredFileNameAsync(string storedFileName)
        {
            return await _context.Attachments
                .AsNoTracking()
                .FirstOrDefaultAsync(attachment => attachment.StoredFileName == storedFileName);
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

        public async Task<(int TicketCount, int InProgressCount, int NeedReviewCount)>
            GetTicketCountsForAnEmployeeAsync(Guid employeeId)
        {
            var counts = await _context.Tickets
                .Where(ticket => ticket.EmployeeId == employeeId)
                .GroupBy(_ => 1)
                .Select(group => new
                {
                    TicketCount = group.Count(ticket =>
                        ticket.TicketStatus != TicketStatus.Done &&
                        ticket.TicketStatus != TicketStatus.Cancelled),
                    InProgressCount = group.Count(ticket =>
                        ticket.TicketStatus == TicketStatus.InProgress),
                    NeedReviewCount = group.Count(ticket =>
                        ticket.TicketStatus == TicketStatus.NeedReview ||
                        ticket.TicketStatus == TicketStatus.InReview)
                })
                .FirstOrDefaultAsync();

            return counts == null
                ? (0, 0, 0)
                : (counts.TicketCount, counts.InProgressCount, counts.NeedReviewCount);
        }

        public async Task<int> GetTicketCompletedCountForAnEmployeeAsync(Guid employeeId)
        {
            return await _context.Tickets.CountAsync(ticket =>
                ticket.EmployeeId == employeeId &&
                ticket.TicketStatus == TicketStatus.Completed);
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
