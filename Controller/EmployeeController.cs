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
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>>
            GetAll()
        {
            var employees =
                await _employeeManager.GetAllAsync();

            return Ok(employees);
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponse>>
            GetById(int id)
        {
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

        [HttpGet("{id}/projects")]
        public async Task<
            ActionResult<IEnumerable<EmployeeProjectResponse>>>
            GetProjects(int id)
        {
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

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeResponse>>
            Update(
                int id,
                [FromBody] UpdateEmployeeRequest request)
        {
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

        [HttpPost("{id:int}/ProfilePhoto")]
        public async Task<ActionResult<EmployeeResponse>> UploadProfilePhoto(
            int id,
            [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    message = "No profile photo was uploaded."
                });

            if (file.Length > MaxProfileImageSizeInBytes)
                return BadRequest(new
                {
                    message = $"Profile photo must be {MaxProfileImageSizeInBytes / 1024 / 1024}MB or smaller."
                });

            if (!AllowedProfileImageContentTypes.Contains(file.ContentType))
                return BadRequest(new
                {
                    message = "Profile photo must be JPG, PNG, GIF, or WEBP."
                });

            if (!Directory.Exists(_profileImagesFolder))
                Directory.CreateDirectory(_profileImagesFolder);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_profileImagesFolder, fileName);

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

        [HttpGet("{id:int}/ProfilePhoto/{fileName}")]
        public IActionResult GetProfilePhoto(int id, string fileName)
        {
            var filePath = Path.Combine(_profileImagesFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new
                {
                    message = "The requested profile photo does not exist."
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

        // =========================================================
        // DELETE
        // =========================================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
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

        [HttpPost("reactivate/{id}")]
        public async Task<IActionResult> Reactivate(int id)
        {
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
