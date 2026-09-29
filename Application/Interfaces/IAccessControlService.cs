namespace ApplicationServices.Interfaces;

public interface IAccessControlService
{
    Task<bool> CanAccessProjectAsync(Guid employeeId, Guid projectId);

    Task<bool> CanManageProjectAsync(Guid employeeId, Guid projectId);

    Task<bool> CanAccessTicketAsync(Guid employeeId, Guid ticketId);

    Task<bool> CanManageTicketAsync(Guid employeeId, Guid ticketId);
}
