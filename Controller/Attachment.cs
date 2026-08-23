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

        public Attachment()
        {
            if (!Directory.Exists(_storageFolder))
                Directory.CreateDirectory(_storageFolder);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null)
                return BadRequest("No file was uploaded");
            string uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(_storageFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            //save this 'uniqueFileName' string into your Database here)
            return Ok(new { fileName = uniqueName, massage = $"Upload successful!{filePath}" });
        }

        [HttpGet("download/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            string filePath = Path.Combine(_storageFolder, fileName);

            // 1. Check if the file physically exists on the disk
            if (!System.IO.File.Exists(filePath))
                return NotFound("The requested file does not exist.");

            // 2. Automatically detect the correct file content type (e.g., image/jpeg, video/mp4)
            string contentType = GetMimeType(filePath);

            // 3. Open the file file-stream
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // 4. Return the file stream; browsers will render images or display video players directly
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
                _ => "application/octet-stream", // Default fallback binary type
            };
        }
    }
}
