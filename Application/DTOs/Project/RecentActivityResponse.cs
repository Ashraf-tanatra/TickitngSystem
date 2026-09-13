namespace ApplicationServices.DTOs.Project
{
    public class RecentActivityResponse
    {
        public int ProjectId { get; set; }
        public int? TicketId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
    }
}
