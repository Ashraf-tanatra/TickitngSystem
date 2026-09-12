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
        [HttpDelete("{id:int}/{empId:int}")]
        [HttpDelete("Delete/{id}/{empId}")]
        public async Task<IActionResult> Delete(int id, int empId)
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
        [HttpGet("ProjectCount/{employeeId:int}")]
        public async Task<ActionResult<int>> ProjectCount(int employeeId)
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
        public async Task<ActionResult<int>> Create([FromBody] CreateProjectRequest request)
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
        [HttpPut("UpdateStatus/{id:int}/{status:int}")]
        public async Task<IActionResult> UpdateStatus(int id, ProjectStatus status)
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
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectResponse>> GetByIdAsync(int id)
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

        }

        //Get api/project/Employees/1
        [HttpGet("Employees/{projectId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployees(int projectId)
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
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/Project/5
        [HttpPut("Update/{id}/{empId}")]
        public async Task<IActionResult> Update(int id, int empId, [FromBody] UpdateProjectRequest request)
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
        [HttpGet("Employee/{employeeId:int}")]
        [HttpGet("employeeId = {employeeId:int}")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsWorkedByEmployee(int employeeId)
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
        }

        [HttpPost("AddEmployee")]
        public async Task<ActionResult<ProjectEmployeeRequest>> AddEmployeeToProject([FromBody] ProjectEmployeeRequest request)
        {
            try
            {
                await _projectManager.ProjectAddEmployeeAsync(request);
                return Ok(request);
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
        [HttpGet("Dashboard/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<string[]>>> GetProjectsWorkedByEmployeeTopThree(int employeeId)
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
        }

        [HttpGet("Dashboard/{employeeId:int}/Projects")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetDashboardProjects(int employeeId)
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

        [HttpGet("Employee/{employeeId:int}/Filter")]
        [HttpGet("employeeId = {employeeId:int}/[controller]")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsWorkedByEmployeeWithFilter(int employeeId,
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
