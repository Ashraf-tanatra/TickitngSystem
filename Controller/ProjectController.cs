using ApplicationServices.DTOs.Project;
using ApplicationServices.DTOs.Ticket;
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

        // DELETE: api/Project/5
        [HttpDelete("Delete/{id}/{empId}")]
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
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Project
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateProjectRequest request)
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
        // need edit: that only manager can update the status to Done or Cancelled or reopen for the project.
        // PUT: api/Project/1/1
        [HttpPut("UpdateStatus/{id:int}/{status:int}")]
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

        //Tested
        // GET: api/Project/1
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

        //Get api/project/Employees/1
        [HttpGet("Employees/{projectId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployees(int projectId)
        {
            //if (!_projectManager.ProjectExits(projectId))
            //    return NotFound();

            var employees = await _projectManager.GetEmployeesWorkOnProjectAsync(projectId)!;
            return Ok(employees);
        }

        //Tested
        // PUT: api/Project/5
        [HttpPut("Update/{id}/{empId}")]
        public async Task<IActionResult> Update(int id, int empId, UpdateProjectRequest request)
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

        //GET api/project/employee = 1
        [HttpGet("employeeId = {employeeId:int}")]
        public async Task<ActionResult<ProjectResponse>> GetProjectsWorkedByEmployee(int employeeId)
        {
            var projcet = _projectManager.GetById(id);
            if (projcet == null)
            {
                return NotFound(projcet);
            }
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

        [Route("AddEmployee")]
        [HttpPost]
        public async Task<ActionResult<ProjectEmployeeRequest>> AddEmployeeToProject(ProjectEmployeeRequest request)
        {
            try
            {
                await _projectManager.ProjectAddEmployeeAsync(request);
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //GET api/project/TopThree/1
        [HttpGet("Dashboard/{employeeId:int}")]
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

        [HttpGet("employeeId = {employeeId:int}/[controller]")]
        public async Task<ActionResult<ProjectResponse>> GetProjectsWorkedByEmployeeWithFilter(int employeeId,
            [FromQuery] ProjectStatus filterStatus)
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


        // GET: api/Project/5/tickets
        [HttpGet("{id}/tickets")]
        public ActionResult<IEnumerable<TicketResponse>> GetTickets(int id)
        {
            try
            {
                var tickets =_projectManager.GetTicketsAsync(id);

                return Ok(tickets);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        //GET: api/Project/5/tickets/10
        //[HttpGet("{projectId}/tickets/{ticketId}")]
        //public ActionResult<TicketResponse> GetTicket(int projectId, int ticketId)
        //{
        //    var ticket = _projectManager.GetTicket(projectId, ticketId);

        //    if (ticket == null)
        //        return NotFound();

        //    return Ok(ticket);
        //}



        // GET: api/Project/5/employees
        //[HttpGet("{id}/employees")]
        //public ActionResult<IEnumerable<EmployeeResponse>> GetEmployees(int id)
        //{
        //    var employees = _projectManager.GetEmployees(id);

        //    return Ok(employees);
        //}
    }
}