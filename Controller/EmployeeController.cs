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

        // =========================================================
        // GET ALL
        // =========================================================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>>
            GetAll()
        {
            var employees =
                await _employeeManager.GetAllAsync();

            return Ok(employees);
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponse>>
            GetById(int id)
        {
            var employee =
                await _employeeManager.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // =========================================================
        // GET PROJECTS
        // =========================================================

        [HttpGet("{id}/projects")]
        public async Task<
            ActionResult<IEnumerable<EmployeeProjectResponse>>>
            GetProjects(int id)
        {
            try
            {
                var projects =
                    await _employeeManager.GetProjectsAsync(id);

                return Ok(projects);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // =========================================================
        // UPDATE
        // =========================================================

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeResponse>>
            Update(
                int id,
                UpdateEmployeeRequest request)
        {
            try
            {
                var employee =
                    await _employeeManager
                        .UpdateAsync(id, request);

                if (employee == null)
                {
                    return NotFound(new
                    {
                        message = Constants.Employee.EmployeeNotFound
                    });
                }

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

        // =========================================================
        // DELETE
        // =========================================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted =
                    await _employeeManager.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = Constants.Employee.EmployeeNotFound
                    });
                }

                return Ok(new
                {
                    message =
                        Constants.Employee.EmployeeDeletedSuccessfully
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

        // =========================================================
        // REACTIVATE EMPLOYEE
        // =========================================================

        [HttpPost("reactivate/{id}")]
        public async Task<IActionResult> Reactivate(int id)
        {
            try
            {
                var result =
                    await _employeeManager.ReactivateAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = Constants.Employee.EmployeeNotFound
                    });
                }

                return Ok(new
                {
                    message =
                        Constants.Employee.EmployeeReactivatedSuccessfully
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

        // =========================================================
        // REACTIVE EMPLOYEE
        // =========================================================
        [HttpPost("reactivate/{id}")]
        public async Task<IActionResult> Reactivate(int id)
        {
            try
            {
                var result =
                    await _employeeManager.ReactivateAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Employee not found."
                    });
                }

                return Ok(new
                {
                    message = "Employee reactivated successfully."
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