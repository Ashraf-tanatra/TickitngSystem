using ApplicationServices.DTOs;
using ApplicationServices.DTOs.ApplicationServices.DTOs;
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

        // =========================
        // POST: api/Auth/signup
        // =========================
        [HttpPost("signup")]
        public async Task<ActionResult<AccountResponse>> SignUp(
            [FromBody] SignUpRequest request)
        {
            try
            {
                var account = await _authManager.SignUp(request);

                return StatusCode(201, account);
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

        // =========================
        // POST: api/Auth/verify-email
        // =========================
        [HttpPost("verify-email")]
        public ActionResult VerifyEmail(
            [FromBody] VerifyEmailRequest request)
        {
            try
            {
                _authManager.VerifyEmail(request);

                return Ok(new
                {
                    message = "Email verified successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // POST: api/Auth/resend-verification-code
        // =========================
        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode(
            [FromBody] ResendVerificationCodeRequest request)
        {
            try
            {
                await _authManager.ResendVerificationCode(request);

                return Ok(new
                {
                    message = "Verification code sent successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // POST: api/Auth/login
        // =========================
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login(
            [FromBody] LoginRequest request)
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