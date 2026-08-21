using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;

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
        public void Add(Ticket ticket)
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
            throw new NotImplementedException();
        }

        public void ChangeTicketStatus(int ticketId, TicketStatus status)
        {
            throw new NotImplementedException();
        }

        public void ChangeTicketPriority(int ticketId, TicketPriority priority)
        {
            throw new NotImplementedException();
        }



        //public IEnumerable<Ticket> GetAll()
        //{
        //    return _context.Tickets.ToList();
        //}
    }
}