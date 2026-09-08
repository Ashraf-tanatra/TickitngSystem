using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class Attachment : ControllerBase
    {
        private readonly string _storageFolder = Path.Combine(Directory.GetCurrentDirectory()
            , "UploadedFiles");
        private const int MaxAttachmentCount = 5;
        private const long MaxAttachmentSizeInBytes = 10 * 1024 * 1024;

        public Attachment()
        {
            if (!Directory.Exists(_storageFolder))
                Directory.CreateDirectory(_storageFolder);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            var validationResult = ValidateAttachment(file);
            if (validationResult != null)
                return validationResult;

            var uploadedFile = await SaveAttachmentAsync(file);
            //save this 'uniqueFileName' string into your Database here)
            return Ok(new
            {
                fileName = uploadedFile.FileName,
                message = "Upload successful."
            });
        }

        [HttpPost("upload/multiple")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
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

            foreach (var file in files)
            {
                var validationResult = ValidateAttachment(file);
                if (validationResult != null)
                    return validationResult;
            }

            var uploadedFiles = new List<object>();

            foreach (var file in files)
            {
                var uploadedFile = await SaveAttachmentAsync(file);
                uploadedFiles.Add(new
                {
                    fileName = uploadedFile.FileName
                });
            }

            return Ok(new
            {
                files = uploadedFiles,
                message = "Upload successful."
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

        private async Task<(string FileName, string FilePath)> SaveAttachmentAsync(IFormFile file)
        {
            string uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(_storageFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (uniqueName, filePath);
        }

        [HttpGet("download/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            string filePath = Path.Combine(_storageFolder, fileName);

            // 1. Check if the file physically exists on the disk
            if (!System.IO.File.Exists(filePath))
                return NotFound(new
                {
                    message = "The requested file does not exist."
                });

            // 2. Automatically detect the correct file content type (e.g., image/jpeg, video/mp4)
            string contentType = GetMimeType(filePath);

            // 3. Open the file file-stream
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // 4. Return the file stream; browsers will render images or display video players directly
            return File(fileStream, contentType, fileName);
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
                _ => "application/octet-stream", // Default fallback binary type
            };
        }
    }
}
