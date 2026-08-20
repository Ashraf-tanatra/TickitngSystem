using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeManager _employeeManager;

        public EmployeeController(IEmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }


        // GET: api/Employee
        [HttpGet]
        public ActionResult<IEnumerable<EmployeeResponse>> GetAll()
        {
            var employees = _employeeManager.GetAll();

            return Ok(employees);
        }


        // GET: api/Employee/5
        [HttpGet("{id}")]
        public ActionResult<EmployeeResponse> GetById(int id)
        {
            var employee = _employeeManager.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }


        // GET: api/Employee/5/projects
        [HttpGet("{id}/projects")]
        public ActionResult<IEnumerable<ProjectResponse>> GetProjects(
            int id)
        {
            try
            {
                var projects = _employeeManager.GetProjects(id);

                return Ok(projects);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public ActionResult<EmployeeResponse> Update(
            int id,
            UpdateEmployeeRequest request)
        {
            try
            {
                var employee =
                    _employeeManager.Update(id, request);

                if (employee == null)
                    return NotFound();

                return Ok(employee);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }


        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _employeeManager.Delete(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}