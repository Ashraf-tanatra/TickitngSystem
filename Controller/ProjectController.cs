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

        //GET api/project/employee = 1
        [HttpGet("employeeId = {employeeId:int}")]
        public ActionResult<ProjectResponse> GetProjectsWorkedByEmployee(int employeeId)
        {
            try
            {
                var projects = _projectManager.GetAllProjectWorkedByEmployee(employeeId);
                if (projects == null)
                    return NotFound();
                return Ok(projects);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        //GET api/project/TopThree/1
        [HttpGet("Dashboard/{employeeId:int}")]
        public ActionResult<ProjectResponse> GetProjectsWorkedByEmployeeTopThree(int employeeId)
        {
            var projects = _projectManager.GetAllProjectWorkedByEmployeeTopThree(employeeId);
            if (projects == null || projects.Count() == 0)
                return NotFound();

            return Ok(projects);
        }

        //Get api/project/Employees/1
        [HttpGet("Employees/{projectId:int}")]
        public ActionResult<IEnumerable<EmployeeResponse>> GetEmployees(int projectId)
        {
            if (!_projectManager.ProjectExits(projectId))
                return NotFound();

            var employees = _projectManager.GetEmployeesWorkOnProject(projectId);
            return Ok(employees);
        }

        //GET api/ProjectCount/1 
        [HttpGet("ProjectCount/{employeeId:int}")]
        public ActionResult<int> ProjectCount(int employeeId)
        {
            return Ok(_projectManager.GetProjectCount(employeeId));
        }

        // GET: api/Project/1
        [HttpGet("{id:int}")]
        public ActionResult<ProjectResponse> GetById(int id)
        {
            if (!_projectManager.ProjectExits(id))
                return NotFound();
            try
            {
                var project = _projectManager.GetById(id);

                return Ok(project);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/Project
        [HttpPost]
        public IActionResult Create(CreateProjectRequest request)
        {
            try
            {
                var projectId = _projectManager.Create(request);

                return Ok(projectId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Project/5
        [HttpPut("Update/{id}/{empId}")]
        public IActionResult Update(int id, int empId, UpdateProjectRequest request)
        {
            try
            {
                _projectManager.Update(id, empId, request);
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
        [HttpDelete("Delete/{id}/{empId}")]
        public IActionResult Delete(int id, int empId)
        {
            try
            {
                if (!_projectManager.ProjectExits(id))
                    return NotFound();

                _projectManager.Delete(id, empId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Project/1/1
        [HttpPut("UpdateStatus/{id:int}/{status:int}")]
        public IActionResult UpdateStatus(int id, ProjectStatus status)
        {

            if (!_projectManager.ProjectExits(id))
            {
                return NotFound();
            }
            try
            {
                _projectManager.SetProjectStatus(id, status);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("AddEmployee")]
        [HttpPost]
        public ActionResult<ProjectEmployeeRequest> AddEmployeeToProject(ProjectEmployeeRequest request)
        {
            try
            {
                _projectManager.ProjectAddEmployee(request);

                return Ok(request);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("employeeId = {employeeId:int}/[controller]")]
        public ActionResult<ProjectResponse> GetProjectsWorkedByEmployeeWithFilter(int employeeId, [FromQuery] ProjectStatus filterStatus)
        {
            try
            {
                var projects = _projectManager.GetAllProjectWorkedByEmployeeWithFilter(employeeId, filterStatus);

                if (projects == null)
                    return NotFound();
                return Ok(projects);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
    }
}