using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectManager _projectManager;

        public ProjectController(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }
        //Tested
        //GET api/project/employee = 1
        [HttpGet("[controller]/employeeId = {employeeId:int}")]
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

        //Tested
        //GET api/project/TopThree/1
        [HttpGet("Dashboard/[controller]/{employeeId:int}")]
        public ActionResult<ProjectResponse> GetProjectsWorkedByEmployeeTopThree(int employeeId)
        {
            //var emp = EmployeeController.GetById(employeeId);
            var projects = _projectManager.GetAllProjectWorkedByEmployeeTopThree(employeeId);
            if (projects == null || projects.Count() == 0)
                return NotFound();

            return Ok(projects);
        }

        //Tested
        //GET api/ProjectCount/1        // include the projects that he manage
        [HttpGet("ProjectCount/{employeeId:int}")]
        public ActionResult<int> ProjectCount(int employeeId)
        {
            return Ok(_projectManager.GetProjectCount(employeeId));
        }

        //Tested
        // GET: api/Project/1
        [HttpGet("[controller]/{id:int}")]
        public ActionResult<ProjectResponse> GetById(int id)
        {
            var project = _projectManager.GetById(id);

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        //Tested
        // POST: api/Project
        [Route("[controller]")]
        [HttpPost]
        public ActionResult<ProjectResponse> Create(CreateProjectRequest request)
        {
            try
            {
                var project = _projectManager.Create(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = project.Id },
                    project);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Tested
        // PUT: api/Project/5
        [HttpPut("[controller]/{id}")]
        public ActionResult<ProjectResponse> Update(int id, UpdateProjectRequest request)
        {
            try
            {
                var project = _projectManager.Update(id, request);

                return Ok(project);
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

        //Tested
        // PUT: api/Project/1/1
        [HttpPut("[controller]/{id:int}/{status:int}")]
        public IActionResult UpdateStatus(int id, ProjectStatus status)
        {
            var projcet = _projectManager.GetById(id);
            if (projcet == null)
            {
                return NotFound(projcet);
            }
            try
            {
                switch (status)
                {
                    case ProjectStatus.Active:
                        _projectManager.SetProjectAsActive(id);
                        break;
                    case ProjectStatus.Completed:
                        _projectManager.SetProjectAsCompleted(id);
                        break;
                    case ProjectStatus.OnHold:
                        _projectManager.SetProjectAsOnHold(id);
                        break;
                    case ProjectStatus.Cancelled:
                        _projectManager.SetProjectAsCancelled(id);
                        break;
                    default:
                        return BadRequest("Invalid status number.");
                }
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


        // Can't delete if there is an employee or an tickets
        // DELETE: api/Project/5
        [HttpDelete("[controller]/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (!_projectManager.Delete(id))
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        // GET: api/Project/5/tickets
        //[HttpGet("{id}/tickets")]
        //public ActionResult<IEnumerable<TicketResponse>> GetTickets(int id)
        //{
        //    var tickets = _projectManager.GetTickets(id);

        //    return Ok(tickets);
        //}

        // GET: api/Project/5/tickets/10
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