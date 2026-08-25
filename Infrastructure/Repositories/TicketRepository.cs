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

        public Ticket? GetById(int id)
        {
            return _context.Tickets.FirstOrDefault(t => t.TicketId == id);
        }
        // =========================================================
        // ADD
        // =========================================================

        public void Create(Ticket ticket)
        {
            _context.Tickets.Add(ticket);

            _context.SaveChanges();
        }
        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);

            _context.SaveChanges();
        }
        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);

            _context.SaveChanges();
        }
        public bool EmployeeExists(int employeeId)
        {
            return _context.Employees.Any(e => e.Id == employeeId);
        }
        public bool ProjectExists(int projectId)
        {
            return _context.Projects.Any(p => p.Id == projectId);
        }

        public IEnumerable<Ticket> GetAllTicketsForAProject(int projectId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ticket> GetAllTicketsForAnEmployee(int employeeId)
        {
            throw new NotImplementedException();
        }

        public int GetTicketTotalCountForAnEmployee(int employeeId)
        {
            throw new NotImplementedException();
        }

        public int GetTicketInProgressCountForAnEmployee(int employeeId)
        {
            throw new NotImplementedException();
        }

  
        public int GetTicketCompletedCountForAnEmployee(int employeeId)
        {
            return _context.Tickets
                .Count(t =>
                    t.EmployeeId == employeeId &&
                    t.TicketStatus == TicketStatus.Completed);
        }

        public void ChangeTicketStatus(int ticketId,TicketStatus status)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            var oldStatus = ticket.TicketStatus;
            var oldEmployeeId = ticket.EmployeeId;

            ticket.TicketStatus = status;

            if (status == TicketStatus.Pending)
            {
                ticket.EmployeeId = null;
            }

            var ManagerId=_context.Projects
                .Where(p => p.Id == ticket.ProjectId)
                .Select(p => p.ProjectManagerId)
                .FirstOrDefault();
            var ticketHistory = new TicketHistory
            {
                TicketId = ticket.TicketId,

                ActionByEmployeeId = ManagerId,

                FromEmployeeId = oldEmployeeId,
                ToEmployeeId = ticket.EmployeeId,

                Action = "Status Changed",

                OldValue = oldStatus.ToString(),
                NewValue = status.ToString(),

                ModifiedAt = DateTime.UtcNow
            };

            _context.TicketHistories.Add(ticketHistory);

             _context.SaveChangesAsync();
        }


        // =========================================================
        // EMPLOYEE BELONGS TO PROJECT
        // =========================================================

        public bool EmployeeBelongsToProject(int employeeId,int projectId)
        {
            return _context.ProjectEmployees
                .Any(pe =>
                    pe.EmployeeId == employeeId &&
                    pe.ProjectId == projectId);
        }

        public void ChangeTicketPriority(int ticketId, TicketPriority priority)
        {
            throw new NotImplementedException();
        }

        public void Add(Ticket ticket)
        {
            throw new NotImplementedException();
        }




        public async Task<IEnumerable<Ticket>> GetByEmployeeAndProjectAsync(   int employeeId,  int projectId)
        {
            return await _context.Tickets.Where(t => t.EmployeeId == employeeId && t.ProjectId == projectId).ToListAsync();
        }
    }
}