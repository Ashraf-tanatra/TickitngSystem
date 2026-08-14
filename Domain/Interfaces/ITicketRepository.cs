using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> GetAll();

        Ticket? GetById(int id);

        IEnumerable<Ticket> GetByProjectId(int projectId);

        void Add(Ticket ticket);

        void Update(Ticket ticket);

        void Delete(Ticket ticket);
    }
}