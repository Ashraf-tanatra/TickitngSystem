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

        public async Task<bool> Delete(int id)
        {
            if (!await _ticketRepository.TicketExists(id))
                return false;

            var ticket = await _ticketRepository.GetById(id);
            await _ticketRepository.Delete(ticket!);
            return await Task.FromResult(true);
        }
        public async Task<TicketResponse?> GetById(int id)
        {
            var ticket = await _ticketRepository.GetById(id);

            if (ticket == null)
                return await Task.FromResult<TicketResponse?>(null);

            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,

                EmployeeId = ticket.EmployeeId,
                EmployeeName = ticket.Employee?.FName + " " + ticket.Employee?.LName,

                ProjectId = ticket.ProjectId,
                ProjectName = ticket.Project?.ProjectName
            };
        }
        public async Task<int> Create(CreateTicketRequest request)
        {
            ArgumentException exception = new ArgumentException("Invalid ticket request.");

            if (request == null)
                throw exception;
            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw exception;
            if (!await _ticketRepository.EmployeeExists(request.EmployeeId))
                throw exception;
            if (!await _ticketRepository.ProjectExists(request.ProjectId))
                throw exception;

            var ticket = new Ticket
            {
                TicketTitle = request.TicketTitle,
                DueTo = request.DueTo,
                Description = request.Description,
                TicketStatus = TicketStatus.Pending,
                Priority = request.Priority,
                EmployeeId = request.EmployeeId,
                ProjectId = request.ProjectId,
                TicketCreatedById = request.TicketCreatedById,
            };
            if (!await _ticketRepository.IsManager(ticket.TicketCreatedById, ticket.ProjectId)) //need fixes
                throw new UnauthorizedAccessException();
            if ((int)ticket.Priority < 0 || (int)ticket.Priority > 2)
                throw new ArgumentException("Priority must be between 0 and 2.");

            await _ticketRepository.Create(ticket);
            return ticket.TicketId;

        }
        public async Task<bool> Update(int id, UpdateTicketRequest request)
        {
            ArgumentException exception = new ArgumentException("Invalid ticket request.");

            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (!await _ticketRepository.TicketExists(id))
                throw exception;
            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw exception;
            if (!await _ticketRepository.EmployeeExists(request.EmployeeId))
                throw exception;

            var ticket = await _ticketRepository.GetById(id);

            if (ticket == null) throw exception;

            ticket.TicketTitle = request.TicketTitle;
            ticket.DueTo = request.DueTo;
            ticket.Description = request.Description;
            ticket.EmployeeId = request.EmployeeId;

            await _ticketRepository.Update(ticket);
            return await Task.FromResult(true);
        }
        public async Task<int> GetTicketTotalCountForAnEmployee(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExists(employeeId))
                throw new ArgumentException("The specified Employee does not exist.");

            return await _ticketRepository.GetTicketTotalCountForAnEmployee(employeeId);
        }
        public async Task<int> GetTicketCompletedCountForAnEmployee(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExists(employeeId))
                throw new ArgumentException("The specified Employee does not exist.");
            return await _ticketRepository.GetTicketCompletedCountForAnEmployee(employeeId);
        }
        public async Task<int> GetTicketInProgressCountForAnEmployee(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExists(employeeId))
                throw new ArgumentException("The specified Employee does not exist.");
            return await _ticketRepository.GetTicketInProgressCountForAnEmployee(employeeId);
        }
        public async Task<bool> AddAttachmentToTicket(int ticketId, string filePath)
        {
            if (!await _ticketRepository.TicketExists(ticketId))
                throw new ArgumentException("The specified Ticket does not exist.");
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.");

            await _ticketRepository.AddAttachmentToTicket(ticketId, filePath);
            return await Task.FromResult(true);
        }
        public async Task<bool> ChangeTicketStatus(int ticketId, TicketStatus status)
        {
            if (!await _ticketRepository.TicketExists(ticketId))
                throw new ArgumentException("The specified Ticket does not exist.");
            if ((int)status < 0 || (int)status > 5)
                throw new ArgumentException("Invalid ticket status.");

            await _ticketRepository.ChangeTicketStatus(ticketId, status);
            return await Task.FromResult(true);
        }
        public async Task<bool> ChangeTicketPriority(int ticketId, TicketPriority priority)
        {
            if (!await _ticketRepository.TicketExists(ticketId))
                throw new ArgumentException("The specified Ticket does not exist.");
            if ((int)priority < 0 || (int)priority > 2)
                throw new ArgumentException("Invalid ticket priority.");

            await _ticketRepository.ChangeTicketPriority(ticketId, priority);
            return await Task.FromResult(true);
        }
        public async Task<IEnumerable<TicketResponse>?> GetAllTicketsForAProject(int projectId)
        {
            if (!await _ticketRepository.ProjectExists(projectId))
                throw new ArgumentException("The specified Project does not exist.");

            var tickets = await _ticketRepository.GetAllTicketsForAProject(projectId);
            if (tickets == null || tickets.Count() == 0)
                throw new ArgumentException("There is no ticket available for the specified project.");

            return tickets.Select(ticket => new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = ticket.EmployeeId,
                EmployeeName = ticket.Employee?.FName + " " + ticket.Employee?.LName,
                ProjectId = ticket.ProjectId,
                ProjectName = ticket.Project?.ProjectName
            });
        }
        public async Task<IEnumerable<TicketResponse>?> GetAllTicketsForAnEmployee(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExists(employeeId))
                throw new ArgumentException("The specified Employee does not exist.");

            var tickets = await _ticketRepository.GetAllTicketsForAnEmployee(employeeId);
            if (tickets == null || tickets.Count() == 0)
                throw new ArgumentException("There is no ticket available for the specified employee.");

            return tickets.Select(ticket => new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = ticket.EmployeeId,
                EmployeeName = ticket.Employee?.FName + " " + ticket.Employee?.LName,
                ProjectId = ticket.ProjectId,
                ProjectName = ticket.Project?.ProjectName
            });
        }

        public async Task<bool> TicketExists(int ticketId) => await _ticketRepository.TicketExists(ticketId);
        public async Task<bool> EmployeeExists(int employeeId) => await _ticketRepository.EmployeeExists(employeeId);
        public async Task<bool> ProjectExists(int projectId) => await _ticketRepository.ProjectExists(projectId);

    }

}