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
        private const int MaxAttachmentCount = 5;
        private const long MaxAttachmentSizeInBytes = 10 * 1024 * 1024;

        public TicketController(ITicketManager ticketManager)
        {
            _ticketManager = ticketManager;
        }


        [HttpGet("Project/{projectId:int}")]
        [HttpGet("/Project/{projectId:int}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTicketsForAProject(int projectId)
        {
            try
            {
                var tickets = await _ticketManager.GetAllTicketsForAProjectAsync(projectId);
                return Ok(tickets);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("Employee/{employeeId:int}")]
        [HttpGet("/Employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTicketsForAnEmployee(int employeeId)
        {
            try
            {
                var tickets = await _ticketManager.GetAllTicketsForAnEmployeeAsync(employeeId);
                return Ok(tickets);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("Employee/{employeeId:int}/TicketCount")]
        [HttpGet("/Employee/{employeeId:int}/TicketCount")]
        public async Task<ActionResult<int>> GetTicketTotalCountForAnEmployee(int employeeId)
        {
            try
            {
                var count = await _ticketManager.GetTicketTotalCountForAnEmployeeAsync(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponse>> GetById(int id)
        {
            var ticket = await _ticketManager.GetByIdAsync(id);

            if (ticket == null)
                return NotFound(new
                {
                    message = "Ticket was not found."
                });

            return Ok(ticket);
        }

        [HttpGet("{ticketId:int}/History")]
        public async Task<ActionResult<IEnumerable<TicketHistoryResponse>>> GetHistory(int ticketId)
        {
            try
            {
                var history = await _ticketManager.GetTicketHistoryAsync(ticketId);
                return Ok(history);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{ticketId:int}/Attachments")]
        public async Task<ActionResult<IEnumerable<TicketAttachmentResponse>>> GetAttachments(int ticketId)
        {
            try
            {
                var attachments = await _ticketManager.GetTicketAttachmentsAsync(ticketId);
                return Ok(attachments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: api/Ticket
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
        {
            try
            {
                var ticketId = await _ticketManager.CreateAsync(request);

                return Ok(ticketId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }
        // PUT: api/Ticket/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketRequest request)
        {
            try
            {
                await _ticketManager.UpdateAsync(id, request);
                return NoContent();
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
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        // DELETE: api/Ticket/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!await _ticketManager.DeleteAsync(id))
                    return NotFound(new
                    {
                        message = "Ticket was not found."
                    });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("Status/{ticketId:int}/{status}")]
        [HttpPut("/Status/{ticketId:int}/{status}")]
        public async Task<IActionResult> ChangeTicketStatus(int ticketId, TicketStatus status)
        {
            try
            {
                await _ticketManager.ChangeTicketStatusAsync(ticketId, status);
                return NoContent();
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
        }

        [HttpPut("Priority/{ticketId:int}/{priority}")]
        [HttpPut("/Priority/{ticketId:int}/{priority}")]
        public async Task<IActionResult> ChangeTicketPriority(int ticketId, TicketPriority priority)
        {
            try
            {
                await _ticketManager.ChangeTicketPriorityAsync(ticketId, priority);
                return NoContent();
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
        }

        [HttpPut("{ticketId:int}/SubmitReview")]
        public async Task<IActionResult> SubmitForReview(int ticketId, [FromBody] TicketActionRequest request)
        {
            try
            {
                await _ticketManager.SubmitForReviewAsync(ticketId, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
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

        [HttpPut("{ticketId:int}/Review/Approve")]
        public async Task<IActionResult> Approve(int ticketId, [FromBody] TicketActionRequest request)
        {
            try
            {
                await _ticketManager.ApproveAsync(ticketId, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
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

        [HttpPut("{ticketId:int}/Review/RequestChanges")]
        public async Task<IActionResult> RequestChanges(int ticketId, [FromBody] TicketActionRequest request)
        {
            try
            {
                await _ticketManager.RequestChangesAsync(ticketId, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
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

        [HttpPut("{ticketId:int}/Review/Reassign")]
        public async Task<IActionResult> Reassign(int ticketId, [FromBody] TicketReassignRequest request)
        {
            try
            {
                await _ticketManager.ReassignAsync(ticketId, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
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

        [HttpPost("{ticketId:int}/Comments")]
        public async Task<IActionResult> AddComment(int ticketId, [FromBody] TicketActionRequest request)
        {
            try
            {
                await _ticketManager.AddCommentAsync(ticketId, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
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


        [HttpGet("Employee/{employeeId:int}/CompletedCount")]
        [HttpGet("/Employee/{employeeId:int}/CompletedCount")]
        public async Task<ActionResult<int>> GetCompletedTicketCountForAnEmployee(int employeeId)
        {
            try
            {
                var count = await _ticketManager.GetTicketCompletedCountForAnEmployeeAsync(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("Employee/{employeeId:int}/InProgressCount")]
        [HttpGet("/Employee/{employeeId:int}/InProgressCount")]
        public async Task<ActionResult<int>> GetInProgressTicketCountForAnEmployee(int employeeId)
        {
            try
            {
                var count = await _ticketManager.GetTicketInProgressCountForAnEmployeeAsync(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("Employee/{employeeId:int}/NeedReviewCount")]
        [HttpGet("/Employee/{employeeId:int}/NeedReviewCount")]
        public async Task<ActionResult<int>> GetNeedReviewTicketCountForAnEmployee(int employeeId)
        {
            try
            {
                var count = await _ticketManager.GetTicketNeedReviewCountForAnEmployeeAsync(employeeId);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }


        //Need Enhancement for directory structure.
        //api/Ticket/Attachments/upload/Ticket/{1}
        [HttpPost("Attachments/upload/Ticket/{ticketId}")]
        public async Task<IActionResult> UploadFile(int ticketId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    message = "No file was uploaded."
                });

            var ticketValidationResult = await ValidateTicketForAttachmentAsync(ticketId);
            if (ticketValidationResult != null)
                return ticketValidationResult;

            var fileValidationResult = ValidateAttachment(file);
            if (fileValidationResult != null)
                return fileValidationResult;

            var uploadedFile = await SaveAttachmentAsync(file);
            await _ticketManager.AddAttachmentToTicketAsync(
                ticketId,
                uploadedFile.Url,
                file.FileName,
                uploadedFile.FileName,
                file.ContentType,
                file.Length);

            return Ok(new
            {
                fileName = uploadedFile.FileName,
                originalFileName = file.FileName,
                contentType = file.ContentType,
                sizeInBytes = file.Length,
                url = uploadedFile.Url,
                message = "Upload successful."
            });
        }

        [HttpPost("Attachments/upload/Ticket/{ticketId}/Multiple")]
        public async Task<IActionResult> UploadFiles(int ticketId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new
                {
                    message = "No files were uploaded."
                });

            if (files.Count > MaxAttachmentCount)
                return BadRequest(new
                {
                    message = $"You can upload up to {MaxAttachmentCount} files at a time."
                });

            var ticketValidationResult = await ValidateTicketForAttachmentAsync(ticketId);
            if (ticketValidationResult != null)
                return ticketValidationResult;

            foreach (var file in files)
            {
                var fileValidationResult = ValidateAttachment(file);
                if (fileValidationResult != null)
                    return fileValidationResult;
            }

            var uploadedFiles = new List<object>();

            foreach (var file in files)
            {
                var uploadedFile = await SaveAttachmentAsync(file);
                await _ticketManager.AddAttachmentToTicketAsync(
                    ticketId,
                    uploadedFile.Url,
                    file.FileName,
                    uploadedFile.FileName,
                    file.ContentType,
                    file.Length);

                uploadedFiles.Add(new
                {
                    fileName = uploadedFile.FileName,
                    originalFileName = file.FileName,
                    contentType = file.ContentType,
                    sizeInBytes = file.Length,
                    url = uploadedFile.Url
                });
            }

            return Ok(new
            {
                files = uploadedFiles,
                message = "Upload successful."
            });
        }

        private async Task<IActionResult?> ValidateTicketForAttachmentAsync(int ticketId)
        {
            if (await _ticketManager.TicketExistsAsync(ticketId))
                return null;

            return NotFound(new
            {
                message = "Ticket was not found."
            });
        }

        private IActionResult? ValidateAttachment(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    message = "No file was uploaded."
                });

            if (file.Length > MaxAttachmentSizeInBytes)
                return BadRequest(new
                {
                    message = $"Each attachment must be {MaxAttachmentSizeInBytes / 1024 / 1024}MB or smaller."
                });

            return null;
        }

        private async Task<(string FileName, string FilePath, string Url)> SaveAttachmentAsync(IFormFile file)
        {
            if (!Directory.Exists(_storageFolder))
                Directory.CreateDirectory(_storageFolder);

            string uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(_storageFolder, uniqueName);
            string url = $"/api/Ticket/Attachments/download-file/{uniqueName}";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (uniqueName, filePath, url);
        }

        [HttpGet("Attachments/download-file/{fileName}")]
        public IActionResult GetFileByName(string fileName)
        {
            string filePath = Path.Combine(_storageFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new
                {
                    message = "The requested file does not exist."
                });

            string contentType = GetMimeType(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, fileName);
        }

        [HttpGet("Attachments/download/{URL}")]
        public IActionResult GetFile(string URL)
        {
            if (!System.IO.File.Exists(URL))
                return NotFound(new
                {
                    message = "The requested file does not exist."
                });

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
