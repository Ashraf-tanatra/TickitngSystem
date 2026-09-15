using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeManager _employeeManager;
        private readonly string _profileImagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", "ProfileImages");
        private const long MaxProfileImageSizeInBytes = 5 * 1024 * 1024;
        private static readonly string[] AllowedProfileImageContentTypes =
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp"
        };

        public EmployeeController(IEmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        // =========================================================
        // GET ALL
        // =========================================================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeSummaryResponse>>>
            GetAll()
        {
            var employees =
                await _employeeManager.GetAllAsync();

            return Ok(employees);
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>>
            GetById(Guid id)
        {
            if (id != User.GetEmployeeId())
                return Forbid();

            var employee =
                await _employeeManager.GetByIdAsync(id);

            if (employee == null)
                return NotFound(new
                {
                    message = ErrorShared.Employee.EmployeeNotFound
                });

            return Ok(employee);
        }

        // =========================================================
        // GET PROJECTS
        // =========================================================

        [HttpGet("{id:guid}/projects")]
        public async Task<
            ActionResult<IEnumerable<EmployeeProjectResponse>>>
            GetProjects(Guid id)
        {
            if (id != User.GetEmployeeId())
                return Forbid();

            try
            {
                var projects =
                    await _employeeManager.GetProjectsAsync(id);

                return Ok(projects);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================================================
        // UPDATE
        // =========================================================

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>>
            Update(
                Guid id,
                [FromBody] UpdateEmployeeRequest request)
        {
            if (id != User.GetEmployeeId())
                return Forbid();

            try
            {
                var employee =
                    await _employeeManager
                        .UpdateAsync(id, request);

                if (employee == null)
                {
                    return NotFound(new
                    {
                        message = ErrorShared.Employee.EmployeeNotFound
                    });
                }

                return Ok(employee);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id:guid}/ProfilePhoto")]
        public async Task<ActionResult<EmployeeResponse>> UploadProfilePhoto(
            Guid id,
            [FromForm] IFormFile file)
        {
            if (id != User.GetEmployeeId())
                return Forbid();

            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    message = ErrorShared.Employee.NoProfileImageUploaded
                });

            if (file.Length > MaxProfileImageSizeInBytes)
                return BadRequest(new
                {
                    message = ErrorShared.Employee.ProfileImageTooLarge(
                        (int)(MaxProfileImageSizeInBytes / 1024 / 1024))
                });

            if (!AllowedProfileImageContentTypes.Contains(file.ContentType))
                return BadRequest(new
                {
                    message = ErrorShared.Employee.InvalidProfileImageType
                });

            if (!Directory.Exists(_profileImagesFolder))
                Directory.CreateDirectory(_profileImagesFolder);

            var extension = GetImageExtension(file.ContentType);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_profileImagesFolder, Path.GetFileName(fileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var profileImageUrl = $"/api/Employee/{id}/ProfilePhoto/{fileName}";

            try
            {
                var employee =
                    await _employeeManager.UpdateProfileImageAsync(id, profileImageUrl);

                if (employee == null)
                    return NotFound(new
                    {
                        message = ErrorShared.Employee.EmployeeNotFound
                    });

                return Ok(employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id:guid}/ProfilePhoto/{fileName}")]
        public IActionResult GetProfilePhoto(Guid id, string fileName)
        {
            var safeFileName = Path.GetFileName(fileName);
            if (!string.Equals(fileName, safeFileName, StringComparison.Ordinal))
                return BadRequest();

            var filePath = Path.Combine(_profileImagesFolder, safeFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new
                {
                    message = ErrorShared.Employee.ProfileImageNotFound
                });

            var contentType = GetImageContentType(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, fileName);
        }

        private static string GetImageContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        private static string GetImageExtension(string contentType) => contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            _ => throw new ArgumentOutOfRangeException(nameof(contentType))
        };

        // =========================================================
        // DELETE
        // =========================================================

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id != User.GetEmployeeId())
                return Forbid();

            try
            {
                var deleted =
                    await _employeeManager.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = ErrorShared.Employee.EmployeeNotFound
                    });
                }

                return Ok(new
                {
                    message =
                        ErrorShared.Employee.EmployeeDeletedSuccessfully
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================================================
        // REACTIVATE EMPLOYEE
        // =========================================================

        [HttpPost("reactivate/{id:guid}")]
        public async Task<IActionResult> Reactivate(Guid id)
        {
            if (id != User.GetEmployeeId())
                return Forbid();

            try
            {
                var result =
                    await _employeeManager.ReactivateAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = ErrorShared.Employee.EmployeeNotFound
                    });
                }

                return Ok(new
                {
                    message =
                        ErrorShared.Employee.EmployeeReactivatedSuccessfully
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        
        
    }
}
