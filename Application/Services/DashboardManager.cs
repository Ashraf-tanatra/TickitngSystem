using ApplicationServices.DTOs.Dashboard;
using ApplicationServices.Interfaces;

namespace ApplicationServices.Services;

public sealed class DashboardManager : IDashboardManager
{
    private readonly IProjectManager _projectManager;
    private readonly ITicketManager _ticketManager;

    public DashboardManager(
        IProjectManager projectManager,
        ITicketManager ticketManager)
    {
        _projectManager = projectManager;
        _ticketManager = ticketManager;
    }

    public async Task<DashboardResponse> GetAsync(Guid employeeId)
    {
        var projectCount = await _projectManager.GetProjectCountAsync(employeeId);
        var ticketCounts = await _ticketManager.GetTicketCountsForAnEmployeeAsync(employeeId);
        var completedCount =
            await _ticketManager.GetTicketCompletedCountForAnEmployeeAsync(employeeId);
        var recentProjects = await _projectManager.GetDashboardProjectsAsync(employeeId);
        var recentTickets =
            await _ticketManager.GetRecentTicketsWithActivityAsync(employeeId);
        var recentActivity = await _projectManager.GetRecentActivityAsync(employeeId);

        return new DashboardResponse
        {
            ProjectCount = projectCount,
            AssignedTicketCount = ticketCounts.TicketCount,
            InProgressCount = ticketCounts.InProgressCount,
            NeedReviewCount = ticketCounts.NeedReviewCount,
            CompletedCount = completedCount,
            RecentProjects = recentProjects.ToArray(),
            RecentTickets = recentTickets.ToArray(),
            RecentActivity = recentActivity.ToArray()
        };
    }
}
