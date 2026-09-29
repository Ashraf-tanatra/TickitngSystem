using ApplicationServices.DTOs.Dashboard;
using ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Controller;

[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardManager _dashboardManager;

    public DashboardController(IDashboardManager dashboardManager)
    {
        _dashboardManager = dashboardManager;
    }

    [HttpGet("me")]
    public async Task<ActionResult<DashboardResponse>> GetMyDashboard()
    {
        try
        {
            return Ok(await _dashboardManager.GetAsync(User.GetEmployeeId()));
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
