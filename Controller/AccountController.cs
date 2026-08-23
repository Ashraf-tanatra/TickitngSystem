using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManager _accountManager;

        public AccountController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }


        // =========================
        // GET ACCOUNT BY EMAIL
        // =========================

        [HttpGet("{email}")]
        public ActionResult<AccountResponse> GetByEmail(string email)
        {
            try
            {
                var account = _accountManager.GetByEmail(email);

                if (account == null)
                    return NotFound(new
                    {
                        message = "Account not found."
                    });

                return Ok(account);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // =========================
        // UPDATE ACCOUNT
        // =========================

        [HttpPut("{id}")]
        public ActionResult<AccountResponse> Update(
            int id,
            UpdateAccountRequest request)
        {
            try
            {
                var account =
                    _accountManager.Update(id, request);

                if (account == null)
                    return NotFound(new
                    {
                        message = "Account not found."
                    });

                return Ok(account);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }


        // =========================
        // DELETE ACCOUNT
        // =========================

        [HttpDelete("{email}")]
        public IActionResult Delete(string email)
        {
            try
            {
                var deleted =
                    _accountManager.Delete(email);

                if (!deleted)
                    return NotFound(new
                    {
                        message = "Account not found."
                    });

                return Ok(new
                {
                    message = "Account deleted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}