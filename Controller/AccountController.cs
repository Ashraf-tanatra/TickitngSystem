using ApplicationServices.DTOs.Account;
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


        // =========================================================
        // Reactivate Account
        // =========================================================
        [HttpPost("reactivate" + "{email}")]
        public IActionResult Reactivate(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new
                    {
                        message = "Email is required."
                    });
                }

                var result = _accountManager.Reactivate(email);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Account not found."
                    });
                }

                return Ok(new
                {
                    message = "Account reactivated successfully."
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

        // =========================
        // UPDATE ACCOUNT
        // =========================

        [HttpPut("{id}")]
        public ActionResult<AccountResponse> Update(int id,UpdateAccountRequest request)
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
        public IActionResult SoftDelete(string email)
        {
            try
            {
                var result = _accountManager.SoftDelete(email);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Account not found."
                    });
                }

                return Ok(new
                {
                    message = "Account deleted successfully. " +
                              "You can reactivate it within 30 days."
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
    }
}