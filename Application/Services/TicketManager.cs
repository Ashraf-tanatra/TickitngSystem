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

        public async Task<bool> Delete(int empId, int ticketId)
        {
            var ticket = await _ticketRepository.GetById(ticketId);
            if (ticket == null)
                throw new ArgumentException("ticket does not exists !!!");
            if (!await _ticketRepository.IsManager(empId, ticket.ProjectId))
                throw new UnauthorizedAccessException("You don't have the authority to delete this ticket");

            return await _ticketRepository.Delete(ticketId);
        }
        public async Task<TicketResponse?> GetById(int ticketId)
        {
            var ticket = await _ticketRepository.GetById(ticketId);

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
                EmployeeName = await _ticketRepository.GetEmpName(ticketId),

                ProjectId = ticket.ProjectId,
                ProjectName = ticket.Project?.ProjectName
            };
        }
        public async Task<int> Create(CreateTicketRequest request, int empCreatedById)
        {
            if (request == null)
                throw new ArgumentException("Invalid ticket request!!!");
            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException("Invalid ticket request ticket title is required!!!");
            if (!Enum.IsDefined(typeof(TicketPriority), request.Priority))
                throw new ArgumentException("Priority must be between 0 and 2 !!!");
            if (empCreatedById == request.EmployeeId)
                throw new ArgumentException("Manager can't get an ticket!!!");

            var empExist = await _ticketRepository.EmployeeExists(request.EmployeeId);
            var projExist = await _ticketRepository.ProjectExists(request.ProjectId);
            var isManager = await _ticketRepository.IsManager(empCreatedById, request.ProjectId);
            var managerExists = await _ticketRepository.EmployeeExists(empCreatedById);

            //await Task.WhenAll(empExist, projExist, isManager);

            if (!empExist || !projExist || !managerExists)
                throw new ArgumentException("The Project or Employee doesn't exists!!!");
            if (!isManager)
                throw new UnauthorizedAccessException("Unauthorized user!!!");

            var ticket = new Ticket
            {
                TicketTitle = request.TicketTitle,
                DueTo = request.DueTo,
                Description = request.Description,
                TicketStatus = TicketStatus.Pending,
                Priority = request.Priority,
                EmployeeId = request.EmployeeId,
                ProjectId = request.ProjectId,
                TicketCreatedById = empCreatedById,
            };

            if (await _ticketRepository.Create(ticket))
                return ticket.TicketId;
            else return 0;
        }
        public async Task<bool> Update(int ticketId, int empCreatedById, UpdateTicketRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException("Invalid ticket request, ticket title is required.");

            var ticket = await _ticketRepository.GetById(ticketId);
            if (ticket == null)
                throw new ArgumentException("Ticket doesn't exists");
            if (!await _ticketRepository.IsManager(empCreatedById, ticket.ProjectId))
                throw new UnauthorizedAccessException("You don't have the authority to update this ticket");
            if (empCreatedById == request.EmployeeId)
                throw new ArgumentException("Manager can't get an ticket!!!");

            ticket.TicketTitle = request.TicketTitle;
            ticket.DueTo = request.DueTo;
            ticket.Description = request.Description;
            ticket.EmployeeId = request.EmployeeId;

            return await _ticketRepository.Update(ticket);
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
        public Task<bool> AddAttachmentToTicket(int ticketId, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.");
            //if (!await _ticketRepository.TicketExists(ticketId))
            //    throw new ArgumentException("The specified Ticket does not exist.");

            return _ticketRepository.AddAttachmentToTicket(ticketId, filePath);
        }
        // Ticket priority can't be changed only by the manager or the employee that whom works on
        public async Task<bool> ChangeTicketStatus(int ticketId, int empId, TicketStatus status)
        {
            if (!Enum.IsDefined(typeof(TicketStatus), status))
                throw new ArgumentException("Invalid ticket status.");
            if (!await _ticketRepository.EmployeeExists(empId))
                throw new ArgumentException("Employee does not exists");

            var ticket = await _ticketRepository.GetById(ticketId);
            if (ticket == null)
                throw new ArgumentException("The specified Ticket does not exist.");
            if (!await _ticketRepository.IsManager(empId, ticket.ProjectId) && ticket.EmployeeId != empId)
                throw new UnauthorizedAccessException("You can't change the status");
            if (ticket.EmployeeId == empId)
            {
                if (status == TicketStatus.Cancelled || status == TicketStatus.Done || status == TicketStatus.Reopened)
                    throw new ArgumentException($"You can't change the status to {status}");
            }
            return await _ticketRepository.ChangeTicketStatus(ticketId, status);
        }
        // Ticket priority can't be changed only by the manager
        public async Task<bool> ChangeTicketPriority(int ticketId, int empCreatedById, TicketPriority priority)
        {
            if (!Enum.IsDefined(typeof(TicketPriority), priority))
                throw new ArgumentException("Invalid ticket priority.");
            if (!await _ticketRepository.EmployeeExists(empCreatedById))
                throw new ArgumentException("employee does not exists!!!");

            var ticket = await _ticketRepository.GetById(ticketId);
            if (ticket == null)
                throw new ArgumentException("The specified Ticket does not exist.");
            if (!await _ticketRepository.IsManager(empCreatedById, ticket.ProjectId))
                throw new ArgumentException("You can't change the status");

            return await _ticketRepository.ChangeTicketPriority(ticketId, priority);
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
                EmployeeName = ticket.Employee != null ? ticket.Employee.FName + " " + ticket.Employee.LName : null,
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
                EmployeeName = ticket.Employee != null ? ticket.Employee.FName + " " + ticket.Employee.LName : null,
                ProjectId = ticket.ProjectId,
                ProjectName = ticket.Project?.ProjectName
            });
        }

        public async Task<bool> TicketExists(int ticketId) => await _ticketRepository.TicketExists(ticketId);
    }

}