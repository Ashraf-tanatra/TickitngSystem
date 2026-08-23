using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketManager _ticketManager;

        public TicketController(ITicketManager ticketManager)
        {
            _ticketManager = ticketManager;
        }


        // =========================================================
        // GET: api/Ticket/5
        // Get Ticket By Id
        // =========================================================
        [HttpGet("{id}")]
        public ActionResult<Ticket> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Ticket id must be greater than 0."
                });
            }

            var ticket = _ticketManager.GetById(id);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            return Ok(ticket);
        }


        // =========================================================
        // GET: api/Ticket/project/5
        // Get All Tickets For Project
        // =========================================================
        [HttpGet("project/{projectId}")]
        public ActionResult<IEnumerable<Ticket>> GetAllTicketsForAProject(
            int projectId)
        {
            if (projectId <= 0)
            {
                return BadRequest(new
                {
                    message = "Project id must be greater than 0."
                });
            }

            if (!_ticketManager.ProjectExists(projectId))
            {
                return NotFound(new
                {
                    message = "Project not found."
                });
            }

            var tickets =
                _ticketManager.GetAllTicketsForAProject(projectId);

            return Ok(tickets);
        }


        // =========================================================
        // GET: api/Ticket/employee/5
        // Get All Tickets For Employee
        // =========================================================
        [HttpGet("employee/{employeeId}")]
        public ActionResult<IEnumerable<Ticket>> GetAllTicketsForAnEmployee(
            int employeeId)
        {
            if (employeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Employee id must be greater than 0."
                });
            }

            if (!_ticketManager.EmployeeExists(employeeId))
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            var tickets =
                _ticketManager.GetAllTicketsForAnEmployee(employeeId);

            return Ok(tickets);
        }


        // =========================================================
        // GET: api/Ticket/employee/5/count
        // Total Tickets
        // =========================================================
        [HttpGet("employee/{employeeId}/count")]
        public ActionResult<int> GetTicketTotalCountForAnEmployee(
            int employeeId)
        {
            if (employeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Employee id must be greater than 0."
                });
            }

            if (!_ticketManager.EmployeeExists(employeeId))
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            var count =
                _ticketManager.GetTicketTotalCountForAnEmployee(employeeId);

            return Ok(count);
        }


        // =========================================================
        // GET: api/Ticket/employee/5/in-progress-count
        // =========================================================
        [HttpGet("employee/{employeeId}/in-progress-count")]
        public ActionResult<int> GetTicketInProgressCountForAnEmployee(
            int employeeId)
        {
            if (employeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Employee id must be greater than 0."
                });
            }

            if (!_ticketManager.EmployeeExists(employeeId))
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            var count =
                _ticketManager.GetTicketInProgressCountForAnEmployee(
                    employeeId);

            return Ok(count);
        }


        // =========================================================
        // GET: api/Ticket/employee/5/completed-count
        // =========================================================
        [HttpGet("employee/{employeeId}/completed-count")]
        public ActionResult<int> GetTicketCompletedCountForAnEmployee(
            int employeeId)
        {
            if (employeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Employee id must be greater than 0."
                });
            }

            if (!_ticketManager.EmployeeExists(employeeId))
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            var count =
                _ticketManager.GetTicketCompletedCountForAnEmployee(
                    employeeId);

            return Ok(count);
        }
        // =========================================================
        // PUT: api/Ticket/5/reassign
        // Reassign Ticket
        // =========================================================

        [HttpPut("{ticketId}/reassign")]
        public IActionResult ReassignTicket(int ticketId,int toEmployeeId,int actionByEmployeeId)
        {
            if (ticketId <= 0)
            {
                return BadRequest(new
                {
                    message = "Ticket id must be greater than 0."
                });
            }

            if (toEmployeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid employee id is required."
                });
            }

            if (actionByEmployeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid action employee id is required."
                });
            }

            try
            {
                _ticketManager.ReassignTicket(
                    ticketId,
                    toEmployeeId,
                    actionByEmployeeId);

                return Ok(new
                {
                    message = "Ticket reassigned successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // =========================================================
        // PUT: api/Ticket/5/status
        // Change Ticket Status
        // =========================================================
        [HttpPut("{ticketId}/status")]
        public IActionResult ChangeTicketStatus(int ticketId,TicketStatus status,int actionByEmployeeId)
        {
            Console.WriteLine("=================================");
            Console.WriteLine($"TicketId: {ticketId}");
            Console.WriteLine($"Status: {status}");
            Console.WriteLine($"Status Number: {(int)status}");
            Console.WriteLine($"ActionByEmployeeId: {actionByEmployeeId}");
            Console.WriteLine("=================================");
            if (ticketId <= 0)
            {
                return BadRequest(new
                {
                    message = "Ticket id must be greater than 0."
                });
            }

            if (actionByEmployeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Action employee id must be greater than 0."
                });
            }

            if (!System.Enum.IsDefined(
                    typeof(TicketStatus),
                    status))
            {
                return BadRequest(new
                {
                    message = "Invalid ticket status."
                });
            }

            var ticket = _ticketManager.GetById(ticketId);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            try
            {
                _ticketManager.ChangeTicketStatus(
                    ticketId,
                    status,
                    actionByEmployeeId);

                return Ok(new
                {
                    message = "Ticket status changed successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // =========================================================
        // PUT: api/Ticket/5/priority
        // Change Ticket Priority
        // =========================================================
        [HttpPut("{ticketId}/priority")]
        public IActionResult ChangeTicketPriority(
            int ticketId,
            TicketPriority priority)
        {
            if (ticketId <= 0)
            {
                return BadRequest(new
                {
                    message = "Ticket id must be greater than 0."
                });
            }

            if (!Enum.IsDefined(typeof(TicketPriority), priority))
            {
                return BadRequest(new
                {
                    message = "Invalid ticket priority."
                });
            }

            var ticket = _ticketManager.GetById(ticketId);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            try
            {
                _ticketManager.ChangeTicketPriority(
                    ticketId,
                    priority);

                return Ok(new
                {
                    message = "Ticket priority changed successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // =========================================================
        // POST: api/Ticket
        // Create Ticket
        // =========================================================
        [HttpPost]
        public ActionResult<Ticket> Add(Ticket ticket)
        {
            if (ticket == null)
            {
                return BadRequest(new
                {
                    message = "Ticket data is required."
                });
            }

            if (string.IsNullOrWhiteSpace(ticket.TicketTitle))
            {
                return BadRequest(new
                {
                    message = "Ticket title is required."
                });
            }

            if (ticket.ProjectId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid project id is required."
                });
            }

            if (ticket.EmployeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid employee id is required."
                });
            }

            if (ticket.TicketCreatedById <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid creator employee id is required."
                });
            }

            if (ticket.DueTo == default)
            {
                return BadRequest(new
                {
                    message = "Due date is required."
                });
            }

            if (ticket.DueTo < DateTime.Now)
            {
                return BadRequest(new
                {
                    message = "Due date cannot be in the past."
                });
            }

            if (!_ticketManager.ProjectExists(ticket.ProjectId))
            {
                return NotFound(new
                {
                    message = "Project not found."
                });
            }

            if (!_ticketManager.EmployeeExists(ticket.EmployeeId))
            {
                return NotFound(new
                {
                    message = "Assigned employee not found."
                });
            }

            if (!_ticketManager.EmployeeExists(ticket.TicketCreatedById))
            {
                return NotFound(new
                {
                    message = "Creator employee not found."
                });
            }

            try
            {
                ticket.CreatedTime = DateTime.UtcNow;

                _ticketManager.Add(ticket);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = ticket.TicketId },
                    ticket);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }


        // =========================================================
        // PUT: api/Ticket/5
        // Update Ticket
        // =========================================================
        [HttpPut("{id}")]
        public ActionResult<Ticket> Update(
            int id,
            Ticket ticket)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Ticket id must be greater than 0."
                });
            }

            if (ticket == null)
            {
                return BadRequest(new
                {
                    message = "Ticket data is required."
                });
            }

            if (ticket.TicketId != 0 &&
                ticket.TicketId != id)
            {
                return BadRequest(new
                {
                    message = "Ticket id in URL does not match ticket id."
                });
            }

            if (string.IsNullOrWhiteSpace(ticket.TicketTitle))
            {
                return BadRequest(new
                {
                    message = "Ticket title is required."
                });
            }

            if (ticket.ProjectId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid project id is required."
                });
            }

            if (ticket.EmployeeId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid employee id is required."
                });
            }

            var existingTicket = _ticketManager.GetById(id);

            if (existingTicket == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            if (!_ticketManager.ProjectExists(ticket.ProjectId))
            {
                return NotFound(new
                {
                    message = "Project not found."
                });
            }

            if (!_ticketManager.EmployeeExists(ticket.EmployeeId))
            {
                return NotFound(new
                {
                    message = "Assigned employee not found."
                });
            }

            try
            {
                ticket.TicketId = id;

                _ticketManager.Update(ticket);

                return Ok(ticket);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }


        // =========================================================
        // DELETE: api/Ticket/5
        // Delete Ticket
        // =========================================================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Ticket id must be greater than 0."
                });
            }

            var ticket = _ticketManager.GetById(id);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            try
            {
                _ticketManager.Delete(ticket);

                return Ok(new
                {
                    message = "Ticket deleted successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}