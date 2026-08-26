using ApplicationServices.DTOs.Ticket;
using ApplicationServices.Interfaces;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketManager _ticketManager;
        private readonly string _storageFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        public TicketController(ITicketManager ticketManager)
        {
            _ticketManager = ticketManager;
        }

        // GET: api/ticket/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponse>> GetById(int id)
        {
            try
            {
                var ticket = await _ticketManager.GetById(id);
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/ticket/projectId = 5
        [HttpGet("/projectId = {projectId}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTicketsForAProject(int projectId)
        {
            try
            {
                var tickets = await _ticketManager.GetAllTicketsForAProject(projectId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/ticket/employee/5
        [HttpGet("/employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTicketsForAnEmployee(int employeeId)
        {
            try
            {
                var tickets = await _ticketManager.GetAllTicketsForAnEmployee(employeeId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/ticket/employee/5/ticketCount
        [HttpGet("/employee/{employeeId}/ticketCount")]
        public async Task<ActionResult<int>> GetTicketTotalCountForAnEmployee(int employeeId)
        {
            try
            {
                var count = await _ticketManager.GetTicketTotalCountForAnEmployee(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/ticket/createTicket
        [HttpPost("createTicket")]
        public async Task<ActionResult> Create(CreateTicketRequest request)
        {
            try
            {
                var ticketId = await _ticketManager.Create(request);
                return Ok(ticketId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/ticket/5/updateTicket
        [HttpPut("{id}/updateTicket")]
        public async Task<IActionResult> Update(int id, UpdateTicketRequest request)
        {
            try
            {
                await _ticketManager.Update(id, request);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/ticket/5/deleteTicket
        [HttpDelete("{id}/deleteTicket")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!await _ticketManager.Delete(id))
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/ticket/changeStatus/5/1
        [HttpPut("/changeStatus/{ticketId}/{status}")]
        public async Task<IActionResult> ChangeTicketStatus(int ticketId, TicketStatus status)
        {
            try
            {
                if (!await _ticketManager.TicketExists(ticketId))
                    return NotFound();

                await _ticketManager.ChangeTicketStatus(ticketId, status);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/ticket/priority/5/0
        [HttpPut("/priority/{ticketId}/{priority}")]
        public async Task<IActionResult> ChangeTicketPriority(int ticketId, TicketPriority priority)
        {
            try
            {
                if (!await _ticketManager.TicketExists(ticketId))
                    return NotFound();

                await _ticketManager.ChangeTicketPriority(ticketId, priority);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/ticket/employee/5/completedCount
        [HttpGet("/employee/{employeeId}/completedCount")]
        public async Task<ActionResult<int>> GetCompletedTicketCountForAnEmployee(int employeeId)
        {
            try
            {
                if (!await _ticketManager.EmployeeExists(employeeId))
                    return NotFound();

                var count = await _ticketManager.GetTicketCompletedCountForAnEmployee(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/ticket/employee/5/inProgressCount
        [HttpGet("/employee/{employeeId}/inProgressCount")]
        public async Task<ActionResult<int>> GetInProgressTicketCountForAnEmployee(int employeeId)
        {
            try
            {
                if (!await _ticketManager.EmployeeExists(employeeId))
                    return NotFound();

                var count = await _ticketManager.GetTicketInProgressCountForAnEmployee(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }


        //Need Enhancement for directory structure.
        //api/ticket/attachments/upload/ticket/{1}
        [HttpPost("attachments/upload/ticket/{ticketId}")]
        public async Task<IActionResult> UploadFile(int ticketId, IFormFile file)
        {
            if (!Directory.Exists(_storageFolder))
                Directory.CreateDirectory(_storageFolder);

            if (file == null)
                return BadRequest("No file was uploaded");

            if (!await _ticketManager.TicketExists(ticketId))
                return NotFound("Ticket does not found.");

            string uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(_storageFolder, uniqueName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            await _ticketManager.AddAttachmentToTicket(ticketId, filePath);
            return Ok(new { fileName = uniqueName, massage = $"Upload successful!{filePath}" });
        }

        [HttpGet("Attachments/download/{URL}")]
        public async Task<IActionResult> GetFile(string URL)
        {
            if (!System.IO.File.Exists(URL))
                return NotFound("The requested file does not exist.");

            try
            {
                string contentType = GetMimeType(URL);
                var fileStream = new FileStream(URL, FileMode.Open, FileAccess.Read);
                return File(fileStream, contentType);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetMimeType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                ".avi" => "video/x-msvideo",
                ".mkv" => "video/x-matroska",
                _ => "application/octet-stream",
            };
        }

    }
}