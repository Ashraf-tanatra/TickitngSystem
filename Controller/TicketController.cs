using ApplicationServices.DTOs;
using Domain.EntityManager;
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

        // GET: api/Ticket
        [HttpGet]
        public ActionResult<IEnumerable<TicketResponse>> GetAll()
        {
            var tickets = _ticketManager.GetAll();

            return Ok(tickets);
        }

        // GET: api/Ticket/5
        [HttpGet("{id}")]
        public ActionResult<TicketResponse> GetById(int id)
        {
            var ticket = _ticketManager.GetById(id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
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
        public ActionResult<TicketResponse> Update(int id,UpdateTicketRequest request)
        {
            try
            {
                var ticket = _ticketManager.Update(id, request);

                return Ok(ticket);
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