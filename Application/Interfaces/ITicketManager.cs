using ApplicationServices.DTOs.Ticket;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        TicketResponse? GetById(int id);

        TicketResponse Create(CreateTicketRequest request);
        TicketResponse Update(int id, UpdateTicketRequest request);
        bool Delete(int id);


        //IEnumerable<TicketResponse> GetAll();
    }
}