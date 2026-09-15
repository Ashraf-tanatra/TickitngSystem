namespace ApplicationServices.DTOs.Ticket;

public sealed class TicketCountsResponse
{
    public int TicketCount { get; set; }

    public int InProgressCount { get; set; }

    public int NeedReviewCount { get; set; }
}
