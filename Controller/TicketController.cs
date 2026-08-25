using ApplicationServices.DTOs.Ticket;
using ApplicationServices.Interfaces;
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

        [HttpGet("{id}")]
        public ActionResult<TicketResponse> GetById(int id)
        {
            var ticket = _ticketManager.GetById(id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        [HttpGet("employee/{employeeId}/project/{projectId}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>>GetByEmployeeAndProject(int employeeId,int projectId)
        {
            var tickets =
                await _ticketManager.GetByEmployeeAndProjectAsync(employeeId,projectId);
            return Ok(tickets);
        }

        // POST: api/Ticket
        [HttpPost]
        public ActionResult<TicketResponse> Create(CreateTicketRequest request)
        {
            try
            {
                var ticket = _ticketManager.Create(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = ticket.TicketId },
                    ticket);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Ticket/5
        [HttpPut("{id}")]
        public ActionResult<TicketResponse> Update( int id,UpdateTicketRequest request)
        {
            try
            {
                _ticketManager.Update(id, request);

                return NoContent();
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

        // DELETE: api/Ticket/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_ticketManager.Delete(id))
                return NotFound();

            return NoContent();
        }
    }




}