using Domain.Entities;

namespace Domain.EntityManager
{
    public interface ITicketManager
    {
        public void CreateNewTicket(Ticket ticket);
        public void UpdateTicket(Ticket ticket);
        public void DeleteTicket(int ticketId);
        public IEnumerable<Ticket> GetAllTicketForProject(int ProjectId);
        public Ticket GetTickedById(int ticketId);
    }
}
