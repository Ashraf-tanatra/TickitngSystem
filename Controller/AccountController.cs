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
        // GET CURRENT ACCOUNT
        // =========================================================

        [HttpGet("me")]
        public async Task<ActionResult<AccountResponse>> GetMyAccount()
        {
            var email = User.GetEmail();

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
        // UPDATE ACCOUNT
        // =========================================================

        [HttpPut("me")]
        public async Task<ActionResult<AccountResponse>> UpdateMyAccount(
            [FromBody] UpdateAccountRequest request)
        {
            var id = User.GetAccountId();

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

        [HttpDelete("deactivate")]
        public async Task<IActionResult> SoftDelete(
            [FromBody] DeactivateAccountRequest request)
        {
            try
            {
                var result =
                    await _accountManager
                        .SoftDeleteAsync(User.GetAccountId(), request);

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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
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
