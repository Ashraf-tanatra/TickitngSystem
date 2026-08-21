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


        // GET: api/Employee (Get All Employee in Database)
        [HttpGet]
        public ActionResult<IEnumerable<EmployeeResponse>> GetAll()
        {
            var employees = _employeeManager.GetAll();

            return Ok(employees);
        }


        // POST: api/Employee/5/reactivate
        [HttpPost("{id}/reactivate")]
        public IActionResult Reactivate(int id,ReactivateAccountRequest request)
        {
            try
            {
                var result = _employeeManager.Reactivate(id, request);

                if (!result)
                    return NotFound(new
                    {
                        message = "Employee not found."
                    });

                return Ok(new
                {
                    message = "Employee reactivated successfully."
                });
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
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
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
        public ActionResult<IEnumerable<EmployeeProjectResponse>> GetProjects(int id)
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
        public ActionResult<EmployeeResponse> Update(int id,UpdateEmployeeRequest request)
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


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var deleted = _employeeManager.Delete(id);

                if (!deleted)
                    return NotFound(new
                    {
                        message = "Employee not found."
                    });

                return Ok(new
                {
                    message = "Employee deleted successfully."
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