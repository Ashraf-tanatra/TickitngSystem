using ApplicationServices.DTOs.Ticket;
using ApplicationServices.Interfaces;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketManager _ticketManager;
        private readonly IAccessControlService _accessControl;
        private readonly string _storageFolder;
        private const int MaxAttachmentCount = 5;
        private const long MaxAttachmentSizeInBytes = 10 * 1024 * 1024;

        public TicketController(
            ITicketManager ticketManager,
            IAccessControlService accessControl,
            IConfiguration configuration)
        {
            _ticketManager = ticketManager;
            _accessControl = accessControl;
            var storageRoot = configuration["FileStorage:RootPath"];
            _storageFolder = string.IsNullOrWhiteSpace(storageRoot)
                ? Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles")
                : Path.GetFullPath(storageRoot);
        }


        [HttpGet("Project/{projectId:guid}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTicketsForAProject(Guid projectId)
        {
            if (!await _accessControl.CanAccessProjectAsync(User.GetEmployeeId(), projectId))
                return Forbid();

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

        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetMyTickets()
        {
            var employeeId = User.GetEmployeeId();

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

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TicketResponse>> GetById(Guid id)
        {
            if (!await _accessControl.CanAccessTicketAsync(User.GetEmployeeId(), id))
                return Forbid();

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
            if (!await _accessControl.CanAccessTicketAsync(User.GetEmployeeId(), ticketId))
                return Forbid();

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
            if (!await _accessControl.CanAccessTicketAsync(User.GetEmployeeId(), ticketId))
                return Forbid();

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
                var ticketId = await _ticketManager.CreateAsync(request, User.GetEmployeeId());

                return Ok(ticketId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        // PUT: api/Ticket/5
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTicketRequest request)
        {
            try
            {
                await _ticketManager.UpdateAsync(id, request, User.GetEmployeeId());
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
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        // DELETE: api/Ticket/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (!await _ticketManager.DeleteAsync(id, User.GetEmployeeId()))
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
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("Status/{ticketId:guid}/{status}")]
        public async Task<IActionResult> ChangeTicketStatus(Guid ticketId, TicketStatus status)
        {
            try
            {
                await _ticketManager.ChangeTicketStatusAsync(ticketId, status, User.GetEmployeeId());
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
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("Priority/{ticketId:guid}/{priority}")]
        public async Task<IActionResult> ChangeTicketPriority(Guid ticketId, TicketPriority priority)
        {
            try
            {
                await _ticketManager.ChangeTicketPriorityAsync(ticketId, priority, User.GetEmployeeId());
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
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{ticketId:guid}/SubmitReview")]
        public async Task<IActionResult> SubmitForReview(Guid ticketId, [FromBody] TicketActionRequest request)
        {
            try
            {
                await _ticketManager.SubmitForReviewAsync(ticketId, request, User.GetEmployeeId());
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
                await _ticketManager.ApproveAsync(ticketId, request, User.GetEmployeeId());
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
                await _ticketManager.RequestChangesAsync(ticketId, request, User.GetEmployeeId());
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
                await _ticketManager.ReassignAsync(ticketId, request, User.GetEmployeeId());
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
                await _ticketManager.AddCommentAsync(ticketId, request, User.GetEmployeeId());
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
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

            var fileValidationResult = await ValidateAttachmentAsync(
                file,
                HttpContext.RequestAborted);
            if (fileValidationResult != null)
                return fileValidationResult;

            var uploadedFile = await SaveAttachmentAsync(file, HttpContext.RequestAborted);
            try
            {
                await _ticketManager.AddAttachmentToTicketAsync(
                    ticketId,
                    uploadedFile.Url,
                    Path.GetFileName(file.FileName),
                    uploadedFile.FileName,
                    file.ContentType,
                    file.Length,
                    User.GetEmployeeId());
            }
            catch
            {
                System.IO.File.Delete(uploadedFile.FilePath);
                throw;
            }

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
                var fileValidationResult = await ValidateAttachmentAsync(
                    file,
                    HttpContext.RequestAborted);
                if (fileValidationResult != null)
                    return fileValidationResult;
            }

            var uploadedFiles = new List<object>();

            foreach (var file in files)
            {
                var uploadedFile = await SaveAttachmentAsync(file, HttpContext.RequestAborted);
                try
                {
                    await _ticketManager.AddAttachmentToTicketAsync(
                        ticketId,
                        uploadedFile.Url,
                        Path.GetFileName(file.FileName),
                        uploadedFile.FileName,
                        file.ContentType,
                        file.Length,
                        User.GetEmployeeId());
                }
                catch
                {
                    System.IO.File.Delete(uploadedFile.FilePath);
                    throw;
                }

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
            if (!await _ticketManager.TicketExistsAsync(ticketId))
            {
                return NotFound(new
                {
                    message = ErrorShared.Ticket.TicketNotFoundMessage
                });
            }

            if (!await _accessControl.CanAccessTicketAsync(User.GetEmployeeId(), ticketId))
                return Forbid();

            return null;
        }

        private async Task<IActionResult?> ValidateAttachmentAsync(
            IFormFile file,
            CancellationToken cancellationToken)
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

            if (!await UploadedFileSecurity.IsSafeAttachmentAsync(file, cancellationToken))
                return BadRequest(new
                {
                    message = ErrorShared.Ticket.InvalidAttachmentType(
                        UploadedFileSecurity.AllowedAttachmentExtensions)
                });

            return null;
        }

        private async Task<(string FileName, string FilePath, string Url)> SaveAttachmentAsync(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (!Directory.Exists(_storageFolder))
                Directory.CreateDirectory(_storageFolder);

            string uniqueName = UploadedFileSecurity.CreateStoredFileName(file.FileName);
            string filePath = Path.Combine(_storageFolder, uniqueName);
            string url = $"/api/Ticket/Attachments/download-file/{uniqueName}";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return (uniqueName, filePath, url);
        }

        [HttpGet("Attachments/download-file/{fileName}")]
        public async Task<IActionResult> GetFileByName(string fileName)
        {
            var safeFileName = Path.GetFileName(fileName);
            var ticketId = await _ticketManager.GetAttachmentTicketIdAsync(safeFileName);
            if (!ticketId.HasValue ||
                !await _accessControl.CanAccessTicketAsync(User.GetEmployeeId(), ticketId.Value))
            {
                return NotFound(new { message = ErrorShared.Ticket.AttachmentNotFound });
            }

            string filePath = Path.Combine(_storageFolder, safeFileName);

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
        public async Task<IActionResult> GetFile(string URL)
        {
            var safeFileName = Path.GetFileName(URL);
            var ticketId = await _ticketManager.GetAttachmentTicketIdAsync(safeFileName);
            if (!ticketId.HasValue ||
                !await _accessControl.CanAccessTicketAsync(User.GetEmployeeId(), ticketId.Value))
            {
                return NotFound(new { message = ErrorShared.Ticket.AttachmentNotFound });
            }

            var filePath = Path.Combine(_storageFolder, safeFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new
                {
                    message = ErrorShared.Ticket.AttachmentNotFound
                });

            string contentType = GetMimeType(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, safeFileName);
        }
        private string GetMimeType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream",
            };
        }

    }
}
