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
                EmployeeName = ticket.Employee?.FName + " " + ticket.Employee?.LName,

                ProjectId = ticket.ProjectId,
                ProjectName = ticket.Project?.ProjectName
            };
        }
        public IEnumerable<TicketResponse>? GetAllTicketsForAProject(int projectId)
        {
            if (!_ticketRepository.ProjectExists(projectId))
                throw new ArgumentException("The specified Project does not exist.");

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
                throw exception;
            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw exception;
            if (!_ticketRepository.EmployeeExists(request.EmployeeId))
                throw exception;
            if (!_ticketRepository.ProjectExists(request.ProjectId))
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
            if (!_ticketRepository.IsManager(ticket.TicketCreatedById, ticket.ProjectId)) //need fixes
                throw new UnauthorizedAccessException();
            if ((int)ticket.Priority < 0 || (int)ticket.Priority > 2)
                throw new ArgumentException("Priority must be between 0 and 2.");

            _ticketRepository.Create(ticket);
            return ticket.TicketId;

        }
        public void Update(int id, UpdateTicketRequest request)
        {
            Exception exception = new Exception();

            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (!_ticketRepository.TicketExists(id))
                throw exception;
            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw exception;
            if (!_ticketRepository.EmployeeExists(request.EmployeeId))
                throw exception;

            var ticket = _ticketRepository.GetById(id);

            ticket.TicketTitle = request.TicketTitle;
            ticket.DueTo = request.DueTo;
            ticket.Description = request.Description;
            ticket.EmployeeId = request.EmployeeId;

            _ticketRepository.Update(ticket);
        }
        public bool Delete(int id)
        {
            if (!_ticketRepository.TicketExists(id))
                return false;

            var ticket = _ticketRepository.GetById(id);
            _ticketRepository.Delete(ticket);
            return true;
        }

        public int GetTicketTotalCountForAnEmployee(int employeeId)
        {
            if (!_ticketRepository.EmployeeExists(employeeId))
                throw new ArgumentException("The specified Employee does not exist.");

            return _ticketRepository.GetTicketTotalCountForAnEmployee(employeeId);
        }
        public int GetTicketInProgressCountForAnEmployee(int employeeId)
        {
            return _ticketRepository.GetTicketInProgressCountForAnEmployee(employeeId);
        }
        public int GetTicketCompletedCountForAnEmployee(int employeeId)
        {
            return _ticketRepository.GetTicketCompletedCountForAnEmployee(employeeId);
        }

        public void ChangeTicketStatus(int ticketId, TicketStatus status)
        {
            _ticketRepository.ChangeTicketStatus(ticketId, status);
        }
        public void ChangeTicketPriority(int ticketId, TicketPriority priority)
        {
            _ticketRepository.ChangeTicketPriority(ticketId, priority);
        }

        public void AddAttachmentToTicket(int ticketId, string filePath)
        {
            _ticketRepository.AddAttachmentToTicket(ticketId, filePath);
        }

        public bool TicketExists(int ticketId)
        {
            return _ticketRepository.TicketExists(ticketId);
        }
        public bool EmployeeExists(int employeeId)
        {
            return _ticketRepository.EmployeeExists(employeeId);
        }
        public bool ProjectExists(int projectId)
        {
            return _ticketRepository.ProjectExists(projectId);
        }
    }

}