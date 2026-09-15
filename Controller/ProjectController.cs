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

        public ProjectController(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        // DELETE: api/Project/Delete/5/1
        [HttpDelete("{id:guid}/{empId:guid}")]
        [HttpDelete("Delete/{id:guid}/{empId:guid}")]
        public async Task<IActionResult> Delete(Guid id, Guid empId)
        {
            try
            {
                await _projectManager.DeleteAsync(id, empId);
                return NoContent();
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

        //GET api/ProjectCount/1 
        [HttpGet("ProjectCount/{employeeId:guid}")]
        public async Task<ActionResult<int>> ProjectCount(Guid employeeId)
        {
            try
            {
                return Ok(await _projectManager.GetProjectCountAsync(employeeId));
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
                var projectId = await _projectManager.CreateAsync(request);
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
                await _projectManager.SetProjectStatusAsync(id, status);
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

        // GET: api/Project/1
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectResponse>> GetByIdAsync(Guid id)
        {
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
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployees(Guid projectId)
        {
            try
            {
                var employees = await _projectManager.GetEmployeesWorkOnProjectAsync(projectId)!;
                return Ok(employees);
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
        [HttpPut("Update/{id:guid}/{empId:guid}")]
        public async Task<IActionResult> Update(Guid id, Guid empId, [FromBody] UpdateProjectRequest request)
        {
            try
            {
                await _projectManager.UpdateAsync(id, empId, request);
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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

        //GET api/Project/Employee/1
        [HttpGet("Employee/{employeeId:guid}")]
        [HttpGet("employeeId = {employeeId:guid}")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsWorkedByEmployee(Guid employeeId)
        {
            try
            {
                var projects = await _projectManager.GetAllProjectWorkedByEmployeeAsync(employeeId)!;
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
                await _projectManager.ProjectAddEmployeeAsync(request);
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

        [HttpDelete("RemoveEmployee")]
        public async Task<IActionResult> RemoveEmployeeFromProject([FromBody] RemoveProjectEmployeeRequest request)
        {
            try
            {
                await _projectManager.RemoveEmployeeFromProjectAsync(request);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
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

        //GET api/project/TopThree/1
        [HttpGet("Dashboard/{employeeId:guid}")]
        public async Task<ActionResult<IEnumerable<string[]>>> GetProjectsWorkedByEmployeeTopThree(Guid employeeId)
        {
            try
            {
                var projects = await _projectManager.GetAllProjectWorkedByEmployeeTopThreeAsync(employeeId)!;
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

        [HttpGet("Dashboard/{employeeId:guid}/Projects")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetDashboardProjects(Guid employeeId)
        {
            try
            {
                var projects = await _projectManager.GetDashboardProjectsAsync(employeeId)!;
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

        // GET: api/Project/RecentActive/1
        [HttpGet("RecentActive/{employeeId:guid}")]
        public async Task<ActionResult<IEnumerable<RecentActivityResponse>>> GetRecentActiveProjects(Guid employeeId)
        {
            try
            {
                var activities = await _projectManager.GetRecentActivityAsync(employeeId);
                return Ok(activities);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("Employee/{employeeId:guid}/Filter")]
        [HttpGet("employeeId = {employeeId:guid}/[controller]")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsWorkedByEmployeeWithFilter(Guid employeeId,
            [FromQuery] ProjectStatus filterStatus)
        {
            try
            {
                var projects = await _projectManager.GetAllProjectWorkedByEmployeeWithFilterAsync(employeeId, filterStatus)!;

                if (projects == null)
                    return NotFound(new
                    {
                        message = ErrorShared.Project.ProjectsNotFound
                    });
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
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
