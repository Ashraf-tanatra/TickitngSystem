using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectManager _projectManager;
        private readonly IAccessControlService _accessControl;

        public ProjectController(IProjectManager projectManager, IAccessControlService accessControl)
        {
            _projectManager = projectManager;
            _accessControl = accessControl;
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentEmployeeId = User.GetEmployeeId();

            try
            {
                await _projectManager.DeleteAsync(id, currentEmployeeId);
                return NoContent();
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

        // POST: api/Project
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateProjectRequest request)
        {
            try
            {
                var projectId = await _projectManager.CreateAsync(request, User.GetEmployeeId());
                return Ok(projectId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpPut("UpdateStatus/{id:guid}/{status:int}")]
        public async Task<IActionResult> UpdateStatus(Guid id, ProjectStatus status)
        {
            try
            {
                await _projectManager.SetProjectStatusAsync(id, status, User.GetEmployeeId());
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

        // GET: api/Project/1
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectResponse>> GetByIdAsync(Guid id)
        {
            if (!await _accessControl.CanAccessProjectAsync(User.GetEmployeeId(), id))
                return Forbid();

            try
            {
                var project = await _projectManager.GetByIdAsync(id);

                if (project == null)
                    return NotFound(new
                    {
                        message = ErrorShared.Project.ProjectNotFound
                    });

                return Ok(project);
            }
            catch (NullReferenceException ex)
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

        //Get api/project/Employees/1
        [HttpGet("Employees/{projectId:guid}")]
        public async Task<ActionResult<IEnumerable<EmployeeSummaryResponse>>> GetEmployees(Guid projectId)
        {
            if (!await _accessControl.CanAccessProjectAsync(User.GetEmployeeId(), projectId))
                return Forbid();

            try
            {
                var employees = await _projectManager.GetEmployeesWorkOnProjectAsync(projectId)!;
                return Ok(employees.Select(employee => new EmployeeSummaryResponse
                {
                    Id = employee.Id,
                    FName = employee.FName,
                    LName = employee.LName,
                    ProfileImageUrl = employee.ProfileImageUrl
                }));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/Project/5
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request)
        {
            var currentEmployeeId = User.GetEmployeeId();

            try
            {
                await _projectManager.UpdateAsync(id, currentEmployeeId, request);
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetMyProjects(
            [FromQuery] ProjectStatus? filterStatus)
        {
            var employeeId = User.GetEmployeeId();

            try
            {
                var projects = filterStatus.HasValue
                    ? await _projectManager.GetAllProjectWorkedByEmployeeWithFilterAsync(
                        employeeId,
                        filterStatus.Value)!
                    : await _projectManager.GetAllProjectWorkedByEmployeeAsync(employeeId)!;
                return Ok(projects);
            }
            catch (NullReferenceException ex)
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

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployeeToProject([FromBody] ProjectEmployeeRequest request)
        {
            try
            {
                await _projectManager.ProjectAddEmployeeAsync(request, User.GetEmployeeId());
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

        [HttpDelete("RemoveEmployee")]
        public async Task<IActionResult> RemoveEmployeeFromProject([FromBody] RemoveProjectEmployeeRequest request)
        {
            try
            {
                await _projectManager.RemoveEmployeeFromProjectAsync(request, User.GetEmployeeId());
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
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

    }
}
