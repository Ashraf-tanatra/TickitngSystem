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

        public AccountController(
            IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        // =========================================================
        // GET ACCOUNT BY EMAIL
        // =========================================================

        [HttpGet("{email}")]
        public async Task<ActionResult<AccountResponse>> GetByEmail(
            string email)
        {
            try
            {
                var account =
                    await _accountManager.GetByEmailAsync(email);

                if (account == null)
                {
                    return NotFound(new
                    {
                        message = ErrorShared.Account.AccountNotFound
                    });
                }

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
        // REACTIVATE ACCOUNT
        // =========================================================

        [HttpPost("reactivate/{email}")]
        public async Task<IActionResult> Reactivate(
            string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new
                    {
                        message = ErrorShared.Account.EmailRequired
                    });
                }

                var result =
                    await _accountManager
                        .ReactivateAsync(email);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = ErrorShared.Account.AccountNotFound
                    });
                }

                return Ok(new
                {
                    message = ErrorShared.Account.AccountReactivatedSuccessfully
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

        // =========================================================
        // UPDATE ACCOUNT
        // =========================================================

        [HttpPut("{id}")]
        public async Task<ActionResult<AccountResponse>> Update(
            int id,
            UpdateAccountRequest request)
        {
            try
            {
                var account =
                    await _accountManager
                        .UpdateAsync(id, request);

                if (account == null)
                {
                    return NotFound(new
                    {
                        message = ErrorShared.Account.AccountNotFound
                    });
                }

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

        // =========================================================
        // SOFT DELETE ACCOUNT
        // =========================================================

        [HttpDelete("{email}")]
        public async Task<IActionResult> SoftDelete(
            string email)
        {
            try
            {
                var result =
                    await _accountManager
                        .SoftDeleteAsync(email);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = ErrorShared.Account.AccountNotFound
                    });
                }

                return Ok(new
                {
                    message = ErrorShared.Account.AccountDeletedSuccessfully
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