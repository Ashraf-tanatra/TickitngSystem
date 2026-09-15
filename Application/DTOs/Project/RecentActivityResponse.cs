namespace ApplicationServices.DTOs.Project
{
    public class RecentActivityResponse
    {
        public Guid ProjectId { get; set; }
        public Guid? TicketId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
    }
}
