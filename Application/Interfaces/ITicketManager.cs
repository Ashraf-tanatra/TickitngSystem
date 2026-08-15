using ApplicationServices.DTOs;

namespace Domain.EntityManager
{
    public interface ITicketManager
    {
        IEnumerable<TicketResponse> GetAll();

        TicketResponse? GetById(int id);

        TicketResponse Create(CreateTicketRequest request);

        TicketResponse Update(
            int id,
            UpdateTicketRequest request);

        bool Delete(int id);
    }
}