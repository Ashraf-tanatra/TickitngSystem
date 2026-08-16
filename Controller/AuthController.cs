using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        // POST: api/Auth/signup
        [HttpPost("signup")]
        public ActionResult<AccountResponse> SignUp(
            SignUpRequest request)
        {
            try
            {
                var account = _authManager.SignUp(request);

                return StatusCode(201, account);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login(
     LoginRequest request)
        {
            try
            {
                var response = _authManager.Login(request);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new
                {
                    message = ex.Message
                });
            }
        }
    }
}