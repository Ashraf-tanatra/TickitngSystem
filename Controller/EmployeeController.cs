using System.Runtime.InteropServices;
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
    }
}
