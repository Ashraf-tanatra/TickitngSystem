using ApplicationServices.DTOs.Ticket;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        Task<TicketResponse?> GetByIdAsync(int id);
        Task<IEnumerable<TicketHistoryResponse>> GetTicketHistoryAsync(int ticketId);
        Task<IEnumerable<TicketAttachmentResponse>> GetTicketAttachmentsAsync(int ticketId);

        Task<IEnumerable<TicketResponse>> GetAllTicketsForAProjectAsync(int projectId);
        Task<IEnumerable<TicketResponse>> GetAllTicketsForAnEmployeeAsync(int employeeId);

        Task<int> CreateAsync(CreateTicketRequest request);
        Task UpdateAsync(int id, UpdateTicketRequest request);
        Task<bool> DeleteAsync(int id);


        Task<int> GetTicketTotalCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketInProgressCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketCompletedCountForAnEmployeeAsync(int employeeId);
        Task<int> GetTicketNeedReviewCountForAnEmployeeAsync(int employeeId);

        Task ChangeTicketStatusAsync(int ticketId, TicketStatus status);
        Task ChangeTicketPriorityAsync(int ticketId, TicketPriority priority);
        Task SubmitForReviewAsync(int ticketId, TicketActionRequest request);
        Task ApproveAsync(int ticketId, TicketActionRequest request);
        Task RequestChangesAsync(int ticketId, TicketActionRequest request);
        Task ReassignAsync(int ticketId, TicketReassignRequest request);
        Task AddCommentAsync(int ticketId, TicketActionRequest request);

        Task AddAttachmentToTicketAsync(
            int ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes);

        Task<bool> TicketExistsAsync(int ticketId);
        Task<bool> EmployeeExistsAsync(int employeeId);
        Task<bool> ProjectExistsAsync(int projectId);
    }
}
