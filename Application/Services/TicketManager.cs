using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;

namespace Domain.EntityManager
{
    public static class ErrorsConstant
    {
        public const string TicketDescriptionIsRequired = "TicketDescriptionIsRequired";

    }
    public class TicketManager : ITicketManager
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEmployeeRepository _employeeRepository;

        
        public TicketManager(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }


        // =========================================================
        // GET BY ID
        // =========================================================
        public Ticket? GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Ticket id must be greater than 0.");

            return _ticketRepository.GetById(id);
        }


        // =========================================================
        // GET ALL TICKETS FOR A PROJECT
        // =========================================================
        public IEnumerable<Ticket> GetAllTicketsForAProject(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException(
                    "Project id must be greater than 0.");

            if (!_ticketRepository.ProjectExists(projectId))
                throw new KeyNotFoundException(
                    "Project not found.");

            return _ticketRepository
                .GetAllTicketsForAProject(projectId);
        }


        // =========================================================
        // GET ALL TICKETS FOR AN EMPLOYEE
        // =========================================================
        public IEnumerable<Ticket> GetAllTicketsForAnEmployee(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException(
                    "Employee id must be greater than 0.");

            if (!_ticketRepository.EmployeeExists(employeeId))
                throw new KeyNotFoundException(
                    "Employee not found.");
            if (_employeeRepository.GetById(employeeId).IsDeleted)
                throw new KeyNotFoundException(
                       "This Employee Is Not Active.");

            return _ticketRepository
                .GetAllTicketsForAnEmployee(employeeId);
        }


        // =========================================================
        // TOTAL TICKET COUNT FOR EMPLOYEE
        // =========================================================
        public int GetTicketTotalCountForAnEmployee(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException(
                    "Employee id must be greater than 0.");

            if (!_ticketRepository.EmployeeExists(employeeId))
                throw new KeyNotFoundException(
                    "Employee not found.");

            return _ticketRepository
                .GetTicketTotalCountForAnEmployee(employeeId);
        }


        // =========================================================
        // IN PROGRESS TICKET COUNT FOR EMPLOYEE
        // =========================================================
        public int GetTicketInProgressCountForAnEmployee(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException(
                    "Employee id must be greater than 0.");

            if (!_ticketRepository.EmployeeExists(employeeId))
                throw new KeyNotFoundException(
                    "Employee not found.");

            return _ticketRepository
                .GetTicketInProgressCountForAnEmployee(employeeId);
        }


        // =========================================================
        // COMPLETED TICKET COUNT FOR EMPLOYEE
        // =========================================================
        public int GetTicketCompletedCountForAnEmployee(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException(
                    "Employee id must be greater than 0.");

            if (!_ticketRepository.EmployeeExists(employeeId))
                throw new KeyNotFoundException(
                    "Employee not found.");

            return _ticketRepository
                .GetTicketCompletedCountForAnEmployee(employeeId);
        }


        // =========================================================
        // CHANGE TICKET STATUS
        // =========================================================
        public void ChangeTicketStatus(
    int ticketId,
    TicketStatus status,
    int actionByEmployeeId)
        {
            if (ticketId <= 0)
                throw new ArgumentException(
                    "Ticket id must be greater than 0.");

            if (!System.Enum.IsDefined(
                    typeof(TicketStatus),
                    status))
            {
                throw new ArgumentException(
                    "Invalid ticket status.");
            }

            if (actionByEmployeeId <= 0)
                throw new ArgumentException(
                    "Valid employee id is required.");

            var ticket = _ticketRepository.GetById(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            if (!_ticketRepository.EmployeeExists(actionByEmployeeId))
                throw new KeyNotFoundException(
                    "Employee who performs the action was not found.");

            var oldStatus = ticket.TicketStatus;

            if (oldStatus == status)
                throw new InvalidOperationException(
                    "Ticket already has this status.");

            _ticketRepository.ChangeTicketStatus(
                ticketId,
                status,
                actionByEmployeeId);
        }
        // =========================================================
        // REASSIGN TICKET
        // =========================================================
        public void ReassignTicket(
    int ticketId,
    int toEmployeeId,
    int actionByEmployeeId)
        {
            if (ticketId <= 0)
                throw new ArgumentException(
                    "Ticket id must be greater than 0.");

            if (toEmployeeId <= 0)
                throw new ArgumentException(
                    "Valid employee id is required.");

            if (actionByEmployeeId <= 0)
                throw new ArgumentException(
                    "Valid action employee id is required.");

            var ticket = _ticketRepository.GetById(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            var fromEmployeeId = ticket.EmployeeId;

            if (fromEmployeeId == toEmployeeId)
                throw new InvalidOperationException(
                    "The ticket is already assigned to this employee.");

            if (!_ticketRepository.EmployeeExists(toEmployeeId))
                throw new KeyNotFoundException(
                    "Employee to assign was not found.");

            if (!_ticketRepository.EmployeeExists(actionByEmployeeId))
                throw new KeyNotFoundException(
                    "Action employee was not found.");

            // New employee must belong to the ticket project
            if (!_ticketRepository.EmployeeBelongsToProject(
                    toEmployeeId,
                    ticket.ProjectId))
            {
                throw new InvalidOperationException(
                    "The employee does not belong to this project.");
            }

            // Person performing the action must belong to the project
            if (!_ticketRepository.EmployeeBelongsToProject(
                    actionByEmployeeId,
                    ticket.ProjectId))
            {
                throw new InvalidOperationException(
                    "The action employee does not belong to this project.");
            }

            _ticketRepository.ReassignTicket(
                ticketId,
                fromEmployeeId,
                toEmployeeId,
                actionByEmployeeId);
        }


        // =========================================================
        // CHANGE TICKET PRIORITY
        // =========================================================
        public void ChangeTicketPriority(
            int ticketId,
            TicketPriority priority)
        {
            if (ticketId <= 0)
                throw new ArgumentException(
                    "Ticket id must be greater than 0.");

            if (!System.Enum.IsDefined(
                    typeof(TicketPriority),
                    priority))
            {
                throw new ArgumentException(
                    "Invalid ticket priority.");
            }

            var ticket = _ticketRepository.GetById(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            _ticketRepository.ChangeTicketPriority(
                ticketId,
                priority);
        }


        // =========================================================
        // ADD
        // =========================================================
        public void Add(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            if (string.IsNullOrWhiteSpace(ticket.TicketTitle))
                throw new ArgumentException(
                    "Ticket title is required.");

            if (string.IsNullOrWhiteSpace(ticket.Description))
                throw new ArgumentException(
                   ErrorsConstant.TicketDescriptionIsRequired);

            if (ticket.ProjectId <= 0)
                throw new ArgumentException(
                    "Valid project id is required.");

            if (ticket.EmployeeId <= 0)
                throw new ArgumentException(
                    "Valid assigned employee id is required.");

            if (ticket.TicketCreatedById <= 0)
                throw new ArgumentException(
                    "Valid creator employee id is required.");

            if (ticket.DueTo == default)
                throw new ArgumentException(
                    "Due date is required.");

            if (!_ticketRepository.ProjectExists(ticket.ProjectId))
                throw new KeyNotFoundException(
                    "Project not found.");

            if (!_ticketRepository.EmployeeExists(ticket.EmployeeId))
                throw new KeyNotFoundException(
                    "Assigned employee not found.");

            if (!_ticketRepository.EmployeeExists(
                    ticket.TicketCreatedById))
            {
                throw new KeyNotFoundException(
                    "Creator employee not found.");
            }

            // Assigned employee must belong to the project
            if (!_ticketRepository.EmployeeBelongsToProject(
                    ticket.EmployeeId,
                    ticket.ProjectId))
            {
                throw new InvalidOperationException(
                    "The assigned employee does not belong to this project.");
            }

            // Creator must belong to the project
            if (!_ticketRepository.EmployeeBelongsToProject(
                    ticket.TicketCreatedById,
                    ticket.ProjectId))
            {
                throw new InvalidOperationException(
                    "The ticket creator does not belong to this project.");
            }

            if (ticket.DueTo < DateTime.Now)
                throw new ArgumentException(
                    "Due date cannot be in the past.");

            _ticketRepository.Add(ticket);
        }


        // =========================================================
        // UPDATE
        // =========================================================
        public void Update(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            if (ticket.TicketId <= 0)
                throw new ArgumentException(
                    "Valid ticket id is required.");

            if (string.IsNullOrWhiteSpace(ticket.TicketTitle))
                throw new ArgumentException(
                    "Ticket title is required.");

            if (string.IsNullOrWhiteSpace(ticket.Description))
                throw new ArgumentException(
                    "Ticket description is required.");

            if (ticket.ProjectId <= 0)
                throw new ArgumentException(
                    "Valid project id is required.");

            if (ticket.EmployeeId <= 0)
                throw new ArgumentException(
                    "Valid assigned employee id is required.");

            if (ticket.DueTo == default)
                throw new ArgumentException(
                    "Due date is required.");

            var existingTicket =
                _ticketRepository.GetById(ticket.TicketId);

            if (existingTicket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            if (!_ticketRepository.ProjectExists(ticket.ProjectId))
                throw new KeyNotFoundException(
                    "Project not found.");

            if (!_ticketRepository.EmployeeExists(ticket.EmployeeId))
                throw new KeyNotFoundException(
                    "Assigned employee not found.");

            // Assigned employee must belong to the project
            if (!_ticketRepository.EmployeeBelongsToProject(
                    ticket.EmployeeId,
                    ticket.ProjectId))
            {
                throw new InvalidOperationException(
                    "The assigned employee does not belong to this project.");
            }

            if (ticket.DueTo < DateTime.Now)
                throw new ArgumentException(
                    "Due date cannot be in the past.");

            _ticketRepository.Update(ticket);
        }


        // =========================================================
        // DELETE
        // =========================================================
        public void Delete(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            if (ticket.TicketId <= 0)
                throw new ArgumentException(
                    "Valid ticket id is required.");

            var existingTicket =
                _ticketRepository.GetById(ticket.TicketId);

            if (existingTicket == null)
                throw new KeyNotFoundException(
                    "Ticket not found.");

            _ticketRepository.Delete(existingTicket);
        }


        // =========================================================
        // EMPLOYEE EXISTS
        // =========================================================
        public bool EmployeeExists(int employeeId)
        {
            if (employeeId <= 0)
                return false;

            return _ticketRepository.EmployeeExists(employeeId);
        }


        // =========================================================
        // PROJECT EXISTS
        // =========================================================
        public bool ProjectExists(int projectId)
        {
            if (projectId <= 0)
                return false;

            return _ticketRepository.ProjectExists(projectId);
        }
    }
}