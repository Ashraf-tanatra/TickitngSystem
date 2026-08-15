using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> GetAll();

        Ticket? GetById(int id);

        void Add(Ticket ticket);

        void Update(Ticket ticket);

        void Delete(Ticket ticket);

        bool EmployeeExists(int employeeId);

        bool ProjectExists(int projectId);
    }
}