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

        public IEnumerable<TicketResponse> GetAll()
        {
            var tickets = _ticketRepository.GetAll();

            return tickets.Select(ticket => new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = ticket.EmployeeId,
                ProjectId = ticket.ProjectId
            });
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
                EmployeeId = ticket.EmployeeId,
                ProjectId = ticket.ProjectId
            };
        }

        public TicketResponse Create(CreateTicketRequest request)
        {
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

            _ticketRepository.Add(ticket);

            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = ticket.EmployeeId,
                ProjectId = ticket.ProjectId
            };
        }

        public TicketResponse Update(
            int id,
            UpdateTicketRequest request)
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
                EmployeeId = ticket.EmployeeId,
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
    }
}