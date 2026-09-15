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


        [HttpGet("Project/{projectId:guid}")]
        [HttpGet("/Project/{projectId:guid}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTicketsForAProject(Guid projectId)
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

        [HttpGet("Employee/{employeeId:guid}")]
        [HttpGet("/Employee/{employeeId:guid}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTicketsForAnEmployee(Guid employeeId)
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

        [HttpGet("Employee/{employeeId:guid}/TicketCount")]
        [HttpGet("/Employee/{employeeId:guid}/TicketCount")]
        public async Task<ActionResult<int>> GetTicketTotalCountForAnEmployee(Guid employeeId)
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
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TicketResponse>> GetById(Guid id)
        {
            var ticket = await _ticketManager.GetByIdAsync(id);

            if (ticket == null)
                return NotFound(new
                {
                    message = ErrorShared.Ticket.TicketNotFoundMessage
                });

            return Ok(ticket);
        }

        [HttpGet("{ticketId:guid}/History")]
        public async Task<ActionResult<IEnumerable<TicketHistoryResponse>>> GetHistory(Guid ticketId)
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

        [HttpGet("{ticketId:guid}/Attachments")]
        public async Task<ActionResult<IEnumerable<TicketAttachmentResponse>>> GetAttachments(Guid ticketId)
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
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTicketRequest request)
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
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (!await _ticketManager.DeleteAsync(id))
                    return NotFound(new
                    {
                        message = ErrorShared.Ticket.TicketNotFoundMessage
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

        [HttpPut("Status/{ticketId:guid}/{status}")]
        [HttpPut("/Status/{ticketId:guid}/{status}")]
        public async Task<IActionResult> ChangeTicketStatus(Guid ticketId, TicketStatus status)
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

        [HttpPut("Priority/{ticketId:guid}/{priority}")]
        [HttpPut("/Priority/{ticketId:guid}/{priority}")]
        public async Task<IActionResult> ChangeTicketPriority(Guid ticketId, TicketPriority priority)
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

        [HttpPut("{ticketId:guid}/SubmitReview")]
        public async Task<IActionResult> SubmitForReview(Guid ticketId, [FromBody] TicketActionRequest request)
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

        [HttpPut("{ticketId:guid}/Review/Approve")]
        public async Task<IActionResult> Approve(Guid ticketId, [FromBody] TicketActionRequest request)
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

        [HttpPut("{ticketId:guid}/Review/RequestChanges")]
        public async Task<IActionResult> RequestChanges(Guid ticketId, [FromBody] TicketActionRequest request)
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

        [HttpPut("{ticketId:guid}/Review/Reassign")]
        public async Task<IActionResult> Reassign(Guid ticketId, [FromBody] TicketReassignRequest request)
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

        [HttpPost("{ticketId:guid}/Comments")]
        public async Task<IActionResult> AddComment(Guid ticketId, [FromBody] TicketActionRequest request)
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


        [HttpGet("Employee/{employeeId:guid}/CompletedCount")]
        [HttpGet("/Employee/{employeeId:guid}/CompletedCount")]
        public async Task<ActionResult<int>> GetCompletedTicketCountForAnEmployee(Guid employeeId)
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

        [HttpGet("Employee/{employeeId:guid}/InProgressCount")]
        [HttpGet("/Employee/{employeeId:guid}/InProgressCount")]
        public async Task<ActionResult<int>> GetInProgressTicketCountForAnEmployee(Guid employeeId)
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

        [HttpGet("Employee/{employeeId:guid}/NeedReviewCount")]
        [HttpGet("/Employee/{employeeId:guid}/NeedReviewCount")]
        public async Task<ActionResult<int>> GetNeedReviewTicketCountForAnEmployee(Guid employeeId)
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
        [HttpPost("Attachments/upload/Ticket/{ticketId:guid}")]
        public async Task<IActionResult> UploadFile(Guid ticketId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    message = ErrorShared.Ticket.NoFileUploaded
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
                message = ErrorShared.Ticket.UploadSuccessful
            });
        }

        [HttpPost("Attachments/upload/Ticket/{ticketId:guid}/Multiple")]
        public async Task<IActionResult> UploadFiles(Guid ticketId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new
                {
                    message = ErrorShared.Ticket.NoFilesUploaded
                });

            if (files.Count > MaxAttachmentCount)
                return BadRequest(new
                {
                    message = ErrorShared.Ticket.TooManyAttachments(MaxAttachmentCount)
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
                message = ErrorShared.Ticket.UploadSuccessful
            });
        }

        private async Task<IActionResult?> ValidateTicketForAttachmentAsync(Guid ticketId)
        {
            if (await _ticketManager.TicketExistsAsync(ticketId))
                return null;

            return NotFound(new
            {
                message = ErrorShared.Ticket.TicketNotFoundMessage
            });
        }

        private IActionResult? ValidateAttachment(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    message = ErrorShared.Ticket.NoFileUploaded
                });

            if (file.Length > MaxAttachmentSizeInBytes)
                return BadRequest(new
                {
                    message = ErrorShared.Ticket.AttachmentTooLarge(
                        (int)(MaxAttachmentSizeInBytes / 1024 / 1024))
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
            string filePath = Path.Combine(_storageFolder, Path.GetFileName(fileName));

            if (!System.IO.File.Exists(filePath))
                return NotFound(new
                {
                    message = ErrorShared.Ticket.AttachmentNotFound
                });

            string contentType = GetMimeType(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, fileName);
        }

        [HttpGet("Attachments/download/{URL}")]
        public IActionResult GetFile(string URL)
        {
            var filePath = Path.Combine(_storageFolder, Path.GetFileName(URL));

            if (!System.IO.File.Exists(filePath))
                return NotFound(new
                {
                    message = ErrorShared.Ticket.AttachmentNotFound
                });

            string contentType = GetMimeType(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
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
