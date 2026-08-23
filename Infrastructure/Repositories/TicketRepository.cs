using Domain.Entities;
using Domain.Enum;
using Domain.Enum;
using Domain.Interfaces;
using Infrastructure.Database;
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


        // =========================================================
        // GET BY ID
        // =========================================================

        public Ticket? GetById(int id)
        {
            return _context.Tickets
                .FirstOrDefault(t => t.TicketId == id);
        }


        // =========================================================
        // GET ALL TICKETS FOR A PROJECT
        // =========================================================

        public IEnumerable<Ticket> GetAllTicketsForAProject(
            int projectId)
        {
            return _context.Tickets
                .Where(t => t.ProjectId == projectId)
                .ToList();
        }


        // =========================================================
        // GET ALL TICKETS FOR AN EMPLOYEE
        // =========================================================

        public IEnumerable<Ticket> GetAllTicketsForAnEmployee(
            int employeeId)
        {
            return _context.Tickets
                .Where(t => t.EmployeeId == employeeId)
                .ToList();
        }


        // =========================================================
        // TOTAL TICKET COUNT FOR EMPLOYEE
        // =========================================================

        public int GetTicketTotalCountForAnEmployee(
            int employeeId)
        {
            return _context.Tickets
                .Count(t => t.EmployeeId == employeeId);
        }


        // =========================================================
        // IN PROGRESS TICKET COUNT
        // =========================================================

        public int GetTicketInProgressCountForAnEmployee(
            int employeeId)
        {
            return _context.Tickets
                .Count(t =>
                    t.EmployeeId == employeeId &&
                    t.TicketStatus == TicketStatus.InProgress);
        }


        // =========================================================
        // COMPLETED TICKET COUNT
        // =========================================================

        public int GetTicketCompletedCountForAnEmployee(
            int employeeId)
        {
            return _context.Tickets
                .Count(t =>
                    t.EmployeeId == employeeId &&
                    t.TicketStatus == TicketStatus.Done);
        }


        // =========================================================
        // CHANGE TICKET STATUS
        // =========================================================

        public void ChangeTicketStatus(
    int ticketId,
    TicketStatus status,
    int actionByEmployeeId)
        {
            var ticket = _context.Tickets
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            var oldStatus = ticket.TicketStatus;

            ticket.TicketStatus = status;

            var history = new TicketHistory
            {
                TicketId = ticketId,
                ActionByEmployeeId = actionByEmployeeId,
                FromEmployeeId = null,
                ToEmployeeId = null,
                Action = "StatusChanged",
                OldValue = oldStatus.ToString(),
                NewValue = status.ToString(),
                CreatedAt = DateTime.Now
            };

            _context.TicketHistories.Add(history);

            _context.SaveChanges();
        }


        // =========================================================
        // CHANGE TICKET PRIORITY
        // =========================================================

        public void ChangeTicketPriority(
            int ticketId,
            TicketPriority priority)
        {
            var ticket = _context.Tickets
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            ticket.Priority = priority;

            _context.SaveChanges();
        }

        // =========================================================
        // REASSIGN
        // =========================================================

        public void ReassignTicket(int ticketId,int fromEmployeeId,int toEmployeeId,int actionByEmployeeId)
        {
            var ticket = _context.Tickets
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            var fromEmployee = _context.Employees
                .FirstOrDefault(e => e.Id == fromEmployeeId);

            var toEmployee = _context.Employees
                .FirstOrDefault(e => e.Id == toEmployeeId);

            if (fromEmployee == null || toEmployee == null)
                throw new KeyNotFoundException(
                    "Employee not found.");

            ticket.EmployeeId = toEmployeeId;

            var history = new TicketHistory
            {
                TicketId = ticketId,
                ActionByEmployeeId = actionByEmployeeId,

                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = toEmployeeId,

                Action = "Reassigned",

                OldValue = $"{fromEmployee.FName} {fromEmployee.LName}",
                NewValue = $"{toEmployee.FName} {toEmployee.LName}",

                CreatedAt = DateTime.Now
            };

            _context.TicketHistories.Add(history);

            _context.SaveChanges();
        }
        // =========================================================
        // ADD
        // =========================================================

        public void Add(Ticket ticket)
        {
            _context.Tickets.Add(ticket);

            _context.SaveChanges();
        }




        // =========================================================
        // UPDATE
        // =========================================================

        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);

            _context.SaveChanges();
        }


        // =========================================================
        // DELETE
        // =========================================================

        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);

            _context.SaveChanges();
        }


        // =========================================================
        // EMPLOYEE EXISTS
        // =========================================================

        public bool EmployeeExists(int employeeId)
        {
            return _context.Employees
                .Any(e =>
                    e.Id == employeeId &&
                    !e.IsDeleted);
        }


        // =========================================================
        // PROJECT EXISTS
        // =========================================================

        public bool ProjectExists(int projectId)
        {
            return _context.Projects
                .Any(p => p.Id == projectId);
        }


        // =========================================================
        // EMPLOYEE BELONGS TO PROJECT
        // =========================================================

        public bool EmployeeBelongsToProject(
            int employeeId,
            int projectId)
        {
            return _context.ProjectEmployees
                .Any(pe =>
                    pe.EmployeeId == employeeId &&
                    pe.ProjectId == projectId);
        }
    }
}