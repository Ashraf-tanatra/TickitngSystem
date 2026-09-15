using ApplicationServices.Interfaces;
using Domain.Interfaces;

namespace ApplicationServices.Services;

public sealed class AccessControlService : IAccessControlService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITicketRepository _ticketRepository;

    public AccessControlService(
        IProjectRepository projectRepository,
        ITicketRepository ticketRepository)
    {
        _projectRepository = projectRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<bool> CanAccessProjectAsync(Guid employeeId, Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        return project != null &&
            (project.ProjectManagerId == employeeId ||
             project.ProjectEmployees.Any(member => member.EmployeeId == employeeId));
    }

    public async Task<bool> CanManageProjectAsync(Guid employeeId, Guid projectId) =>
        await _projectRepository.IsManagerAsync(projectId, employeeId);

    public async Task<bool> CanAccessTicketAsync(Guid employeeId, Guid ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        return ticket != null && await CanAccessProjectAsync(employeeId, ticket.ProjectId);
    }

    public async Task<bool> CanManageTicketAsync(Guid employeeId, Guid ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        return ticket != null && await CanManageProjectAsync(employeeId, ticket.ProjectId);
    }
}
