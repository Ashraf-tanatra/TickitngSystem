using ApplicationServices.DTOs.Ticket;
using ApplicationServices.Interfaces;
using Domain.Entities;
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

        public TicketResponse? GetById(int id)
        {
            var ticket = _ticketRepository.GetById(id);

            if (ticket == null)
                return null;

            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = (int)ticket.EmployeeId,
                ProjectId = ticket.ProjectId
            };
        }

            var tickets = _ticketRepository.GetAllTicketsForAProject(projectId);
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
        public IEnumerable<TicketResponse>? GetAllTicketsForAnEmployee(int employeeId)
        {
            if (!_ticketRepository.EmployeeExists(employeeId))
                throw new ArgumentException("The specified Employee does not exist.");

            var tickets = _ticketRepository.GetAllTicketsForAnEmployee(employeeId);
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

        public int Create(CreateTicketRequest request)
        {
            Exception exception = new ArgumentException("Invalid ticket request.");

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException(
                    "Ticket title is required.");

            if (!_ticketRepository.EmployeeExists(request.EmployeeId))
                throw new ArgumentException(
                    "The specified Employee does not exist.");

            if (!_ticketRepository.ProjectExists(request.ProjectId))
                throw new ArgumentException(
                    "The specified Project does not exist.");

            var ticket = new Ticket
            {
                TicketTitle = request.TicketTitle,
                DueTo = request.DueTo,
                Description = request.Description,
                Priority = request.Priority,
                EmployeeId = request.EmployeeId,
                ProjectId = request.ProjectId
            };
            if (!_ticketRepository.IsManager(ticket.TicketCreatedById, ticket.ProjectId)) //need fixes
                throw new UnauthorizedAccessException();
            if ((int)ticket.Priority < 0 || (int)ticket.Priority > 2)
                throw new ArgumentException("Priority must be between 0 and 2.");

            _ticketRepository.Add(ticket);

            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = (int)ticket.EmployeeId,
                ProjectId = ticket.ProjectId
            };
        }

        public TicketResponse Update(int id, UpdateTicketRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var ticket = _ticketRepository.GetById(id);

            if (ticket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException(
                    "Ticket title is required.");

            if (!_ticketRepository.EmployeeExists(request.EmployeeId))
                throw new ArgumentException(
                    "The specified Employee does not exist.");

            ticket.TicketTitle = request.TicketTitle;
            ticket.DueTo = request.DueTo;
            ticket.Description = request.Description;
            ticket.EmployeeId = request.EmployeeId;

            _ticketRepository.Update(ticket);

            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = (int)ticket.EmployeeId,
                ProjectId = ticket.ProjectId
            };
        }

        public bool Delete(int id)
        {
            var ticket = _ticketRepository.GetById(id);

            if (ticket == null)
                return false;

            _ticketRepository.Delete(ticket);

            return true;
        }
        public async Task<IEnumerable<TicketResponse>> GetByEmployeeAndProjectAsync(int employeeId,int projectId)
        {
            // validation/business rules here
            var tickets =await _ticketRepository.GetByEmployeeAndProjectAsync(employeeId,projectId);
            return MapToResponse(tickets);
        }

        private IEnumerable<TicketResponse> MapToResponse(
      IEnumerable<Ticket> tickets)
        {
            return tickets.Select(ticket => new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = (int)ticket.EmployeeId,
                ProjectId = ticket.ProjectId
            });
        }

    }

   

}
