using ApplicationServices.DTOs.Project;
using ApplicationServices.DTOs.Ticket;

namespace ApplicationServices.DTOs.Dashboard;

public sealed class DashboardResponse
{
    public int ProjectCount { get; set; }

    public int AssignedTicketCount { get; set; }

    public int InProgressCount { get; set; }

    public int NeedReviewCount { get; set; }

    public int CompletedCount { get; set; }

    public IReadOnlyCollection<ProjectResponse> RecentProjects { get; set; } =
        Array.Empty<ProjectResponse>();

    public IReadOnlyCollection<TicketResponse> RecentTickets { get; set; } =
        Array.Empty<TicketResponse>();

    public IReadOnlyCollection<RecentActivityResponse> RecentActivity { get; set; } =
        Array.Empty<RecentActivityResponse>();
}
