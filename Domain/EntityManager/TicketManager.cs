using Domain.Entities;

namespace Domain.EntityManager
{
    public class TicketManager : ITicketManager
    {
        public void CreateNewTicket(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public void DeleteTicket(int ticketId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ticket> GetAllTicketForProject(int ProjectId)
        {
            throw new NotImplementedException();
        }

        public Ticket GetTickedById(int ticketId)
        {
            throw new NotImplementedException();
        }

        public void UpdateTicket(Ticket ticket)
        {
            throw new NotImplementedException();
        }
    }
}
