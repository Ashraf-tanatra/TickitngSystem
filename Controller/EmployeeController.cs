using ApplicationServices.DTOs.Employee;
using ApplicationServices.DTOs.Project;
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

        [HttpGet("{id}/projects")]
        public ActionResult<IEnumerable<ProjectResponse>> GetProjects(int id)
        {
            var projects = _employeeManager.GetProjects(id);

            return Ok(projects);

        }

        // POST: api/Employee
        [HttpPost]
        public ActionResult<EmployeeResponse> Create(CreateEmployeeRequest request)
        {
            var employee = _employeeManager.Create(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = employee.Id },
                employee);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public ActionResult<EmployeeResponse> Update(int id, UpdateEmployeeRequest request)
        {
            var employee = _employeeManager.Update(id, request);

            if (employee == null)
                return NotFound();

            return Ok(employee);
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