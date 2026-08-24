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


        [HttpGet("/Project/{projectId}")]
        public ActionResult<IEnumerable<TicketResponse>> GetAllTicketsForAProject(int projectId)
        {
            try
            {
                var tickets = _ticketManager.GetAllTicketsForAProject(projectId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/Employee/{employeeId}")]
        public ActionResult<IEnumerable<TicketResponse>> GetAllTicketsForAnEmployee(int employeeId)
        {
            try
            {
                var tickets = _ticketManager.GetAllTicketsForAnEmployee(employeeId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/Employee/{employeeId}/TicketCount")]
        public ActionResult<int> GetTicketTotalCountForAnEmployee(int employeeId)
        {
            try
            {
                var count = _ticketManager.GetTicketTotalCountForAnEmployee(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
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
        public IActionResult Create(CreateTicketRequest request)
        {
            try
            {
                var ticketId = _ticketManager.Create(request);

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
        // PUT: api/Ticket/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateTicketRequest request)
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
            catch (Exception ex)
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

        [HttpPut("/Status/{ticketId}/{status}")]
        public IActionResult ChangeTicketStatus(int ticketId, TicketStatus status)
        {
            try
            {
                if (!_ticketManager.TicketExists(ticketId))
                    return NotFound();

                _ticketManager.ChangeTicketStatus(ticketId, status);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("/Priority/{ticketId}/{priority}")]
        public IActionResult ChangeTicketPriority(int ticketId, TicketPriority priority)
        {
            try
            {
                if (!_ticketManager.TicketExists(ticketId))
                    return NotFound();

                _ticketManager.ChangeTicketPriority(ticketId, priority);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("/Employee/{employeeId}/CompletedCount")]
        public ActionResult<int> GetCompletedTicketCountForAnEmployee(int employeeId)
        {
            try
            {
                if (!_ticketManager.EmployeeExists(employeeId))
                    return NotFound();

                var count = _ticketManager.GetTicketCompletedCountForAnEmployee(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("/Employee/{employeeId}/InProgressCount")]
        public ActionResult<int> GetInProgressTicketCountForAnEmployee(int employeeId)
        {
            try
            {
                if (!_ticketManager.EmployeeExists(employeeId))
                    return NotFound();

                var count = _ticketManager.GetTicketInProgressCountForAnEmployee(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }


        //Need Enhancement for directory structure.
        //api/Ticket/Attachments/upload/Ticket/{1}
        [HttpPost("Attachments/upload/Ticket/{ticketId}")]
        public async Task<IActionResult> UploadFile(int ticketId, IFormFile file)
        {
            if (!Directory.Exists(_storageFolder))
                Directory.CreateDirectory(_storageFolder);

            if (file == null)
                return BadRequest("No file was uploaded");

            if (!_ticketManager.TicketExists(ticketId))
                return NotFound("Ticket does not found.");

            string uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(_storageFolder, uniqueName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            _ticketManager.AddAttachmentToTicket(ticketId, filePath);
            return Ok(new { fileName = uniqueName, massage = $"Upload successful!{filePath}" });
        }

        [HttpGet("Attachments/download/{URL}")]
        public IActionResult GetFile(string URL)
        {
            if (!System.IO.File.Exists(URL))
                return NotFound("The requested file does not exist.");

            string contentType = GetMimeType(URL);
            var fileStream = new FileStream(URL, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType);
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