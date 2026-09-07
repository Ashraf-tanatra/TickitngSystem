using ApplicationServices.DTOs.Ticket;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class TicketManager : ITicketManager
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketManager(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<TicketResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket == null)
                return null;

            return MapToResponse(ticket);
        }

        public async Task<IEnumerable<TicketResponse>> GetAllTicketsForAProjectAsync(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            if (!await _ticketRepository.ProjectExistsAsync(projectId))
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            var tickets = await _ticketRepository.GetAllTicketsForAProjectAsync(projectId);
            if (!tickets.Any())
                throw new ArgumentException(ErrorShared.Ticket.NoTicketsForProject);

            return tickets.Select(MapToResponse);
        }

        public async Task<IEnumerable<TicketResponse>> GetAllTicketsForAnEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            var tickets = await _ticketRepository.GetAllTicketsForAnEmployeeAsync(employeeId);
            if (!tickets.Any())
                throw new ArgumentException(ErrorShared.Ticket.NoTicketsForEmployee);

            return tickets.Select(MapToResponse);
        }

        public async Task<int> CreateAsync(CreateTicketRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException(ErrorShared.Ticket.TicketTitleRequired);

            if (!await _ticketRepository.EmployeeExistsAsync(request.EmployeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.EmployeeExistsAsync(request.TicketCreatedById))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.ProjectExistsAsync(request.ProjectId))
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            var ticket = Ticket.Create(
                request.TicketTitle,
                request.DueTo,
                request.Description,
                request.Priority,
                request.ProjectId,
                request.EmployeeId,
                request.TicketCreatedById);

            if (!await _ticketRepository.IsManagerAsync(ticket.TicketCreatedById, ticket.ProjectId))
                throw new UnauthorizedAccessException(ErrorShared.Ticket.UnauthorizedTicketCreation);

            await _ticketRepository.CreateAsync(ticket);
            return ticket.TicketId;

        }

        public async Task UpdateAsync(int id, UpdateTicketRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!await _ticketRepository.TicketExistsAsync(id))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException(ErrorShared.Ticket.TicketTitleRequired);

            if (!await _ticketRepository.EmployeeExistsAsync(request.EmployeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            var ticket = await _ticketRepository.GetByIdAsync(id);

            ticket!.UpdateDetails(
                request.TicketTitle,
                request.DueTo,
                request.Description,
                request.EmployeeId);

            await _ticketRepository.UpdateAsync(ticket);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (!await _ticketRepository.TicketExistsAsync(id))
                return false;

            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket is null)
            {
                return false;
            }

            await _ticketRepository.DeleteAsync(ticket);
            return true;
        }

        public async Task<int> GetTicketTotalCountForAnEmployeeAsync(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            return await _ticketRepository.GetTicketTotalCountForAnEmployeeAsync(employeeId);
        }

        public async Task<int> GetTicketInProgressCountForAnEmployeeAsync(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            return await _ticketRepository.GetTicketInProgressCountForAnEmployeeAsync(employeeId);
        }

        public async Task<int> GetTicketCompletedCountForAnEmployeeAsync(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            return await _ticketRepository.GetTicketCompletedCountForAnEmployeeAsync(employeeId);
        }

        public async Task ChangeTicketStatusAsync(int ticketId, TicketStatus status)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            await _ticketRepository.ChangeTicketStatusAsync(ticketId, status);
        }

        public async Task ChangeTicketPriorityAsync(int ticketId, TicketPriority priority)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            await _ticketRepository.ChangeTicketPriorityAsync(ticketId, priority);
        }

        public async Task AddAttachmentToTicketAsync(int ticketId, string filePath)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            await _ticketRepository.AddAttachmentToTicketAsync(ticketId, filePath);
        }

        public async Task<bool> TicketExistsAsync(int ticketId)
        {
            return await _ticketRepository.TicketExistsAsync(ticketId);
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _ticketRepository.EmployeeExistsAsync(employeeId);
        }

        public async Task<bool> ProjectExistsAsync(int projectId)
        {
            return await _ticketRepository.ProjectExistsAsync(projectId);
        }

        private static TicketResponse MapToResponse(Ticket ticket)
        {
            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = ticket.EmployeeId,
                EmployeeName = ticket.Employee == null
                    ? null
                    : $"{ticket.Employee.FName} {ticket.Employee.LName}",
                ProjectId = ticket.ProjectId,
                ProjectName = ticket.Project?.ProjectName
            };
        }
    }

}
