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

        // GET: api/project/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectResponse>> GetByIdAsync(int id)
        {
            if (id.GetType() != typeof(int))
                return NotFound("The project id must be an integer");
            try
            {
                var project = await _projectManager.GetByIdAsync(id);
                return Ok(project);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }

        }

        //GET api/project/employee = 1
        [HttpGet("employeeId = {employeeId:int}")]
        public async Task<ActionResult<ProjectResponse>> GetProjectsWorkedByEmployee(int employeeId)
        {
            try
            {
                var projects = await _projectManager.GetAllProjectWorkedByEmployeeAsync(employeeId)!;
                return Ok(projects);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        //GET api/project/dashboard/employeeId = 1
        [HttpGet("dashboard/employeeId = {employeeId:int}")]
        public async Task<ActionResult<ProjectResponse>> GetProjectsWorkedByEmployeeTopThree(int employeeId)
        {
            try
            {
                var projects = await _projectManager.GetAllProjectWorkedByEmployeeTopThreeAsync(employeeId)!;
                return Ok(projects);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        //Get api/project/1/Employees
        [HttpGet("{projectId:int}/Employees")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployees(int projectId)
        {
            //if (!_projectManager.ProjectExits(projectId))
            //    return NotFound();

            var employees = await _projectManager.GetEmployeesWorkOnProjectAsync(projectId)!;
            return Ok(employees);
        }

        //GET api/projectCount/EmployeeId = 1
        [HttpGet("projectCount/EmployeeId = {employeeId:int}")]
        public async Task<ActionResult<int>> ProjectCount(int employeeId)
        {
            try
            {
                return Ok(await _projectManager.GetProjectCountAsync(employeeId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/project/CreateProject
        [HttpPost("CreateProject")]
        public async Task<ActionResult<int>> Create(CreateProjectRequest request) // , employeeId
        {
            try
            {
                var projectId = await _projectManager.CreateAsync(request);
                return Ok(projectId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/project/5
        [HttpPut("Update/employeeId = {empId:int}/projectId = {id:int}")]
        public async Task<IActionResult> Update(int empId, int id, UpdateProjectRequest request)
        {
            try
            {
                await _projectManager.UpdateAsync(id, empId, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Project/5
        [HttpDelete("Delete/projectId = {id:int}/employeeId = {empId:int}")]
        public async Task<IActionResult> Delete(int id, int empId)
        {
            try
            {
                //if (!_projectManager.ProjectExits(id))
                //    return NotFound();

                await _projectManager.DeleteAsync(id, empId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddEmployee")] // add employeeId and projectId
        public async Task<ActionResult<ProjectEmployeeRequest>> AddEmployeeToProject(ProjectEmployeeRequest request)
        {
            try
            {
                await _projectManager.ProjectAddEmployeeAsync(request);
                return Ok(request);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // need edit: that only manager can update the status to Done or Cancelled or reopen for the project.
        // PUT: api/Project/changeStatus/1/1
        [HttpPut("changeStatus/projectId = {id:int}/status = {status:int}")]
        public async Task<IActionResult> UpdateStatus(int id, ProjectStatus status)
        {

            //if (!_projectManager.ProjectExits(id))
            //{
            //    return NotFound();
            //}
            try
            {
                await _projectManager.SetProjectStatusAsync(id, status);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("employeeId = {employeeId:int}/filterByStatus")]
        public async Task<ActionResult<ProjectResponse>> GetProjectsWorkedByEmployeeWithFilter(int employeeId,
            [FromQuery] ProjectStatus filterStatus) // need some enhancements with 0 project filtering
        {
            try
            {
                var projects = await _projectManager.GetAllProjectWorkedByEmployeeWithFilterAsync(employeeId, filterStatus)!;

                if (projects == null)
                    return NotFound();
                return Ok(projects);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

    }
}