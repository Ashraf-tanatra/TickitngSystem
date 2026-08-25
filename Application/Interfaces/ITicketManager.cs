using ApplicationServices.DTOs.Ticket;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        TicketResponse? GetById(int id);

        TicketResponse Create(CreateTicketRequest request);
        TicketResponse Update(int id, UpdateTicketRequest request);
        bool Delete(int id);
       Task<IEnumerable<TicketResponse>>GetByEmployeeAndProjectAsync(int employeeId,int projectId);

        //IEnumerable<TicketResponse> GetAll();
    }
}