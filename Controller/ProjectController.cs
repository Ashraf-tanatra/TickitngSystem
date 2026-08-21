using ApplicationServices.DTOs;
using Domain.EntityManager;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {

        // POST: api/Project
        [HttpPost]
        public ActionResult<ProjectResponse> Create(CreateProjectRequest request)
        {
            try
            {
                var project = _projectManager.Create(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = project.Id },
                    project);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Project/5
        [HttpPut("{id}")]
        public ActionResult<ProjectResponse> Update( int id, UpdateProjectRequest request)
        {
            try
            {
                var project = _projectManager.Update(id, request);

                return Ok(project);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Project/5/tickets
        [HttpGet("{id}/tickets")]
        public ActionResult<IEnumerable<TicketResponse>> GetTickets(int id)
        {
            var tickets = _projectManager.GetTickets(id);

            return Ok(tickets);
        }

        // GET: api/Project/5/tickets/10
        [HttpGet("{projectId}/tickets/{ticketId}")]
        public ActionResult<TicketResponse> GetTicket(int projectId, int ticketId)
        {
            var ticket = _projectManager.GetTicket(projectId, ticketId);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        // DELETE: api/Project/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_projectManager.Delete(id))
                return NotFound();

            return NoContent();
        }

        // GET: api/Project/5/employees
        [HttpGet("{id}/employees")]
        public ActionResult<IEnumerable<EmployeeResponse>> GetEmployees(int id)
        {
            var employees = _projectManager.GetEmployees(id);

            return Ok(employees);
        }
    }
}