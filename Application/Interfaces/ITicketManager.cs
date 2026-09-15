using ApplicationServices.DTOs.Ticket;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        Task<TicketResponse?> GetByIdAsync(Guid id);
        Task<IEnumerable<TicketHistoryResponse>> GetTicketHistoryAsync(Guid ticketId);
        Task<IEnumerable<TicketAttachmentResponse>> GetTicketAttachmentsAsync(Guid ticketId);

        Task<IEnumerable<TicketResponse>> GetAllTicketsForAProjectAsync(Guid projectId);
        Task<IEnumerable<TicketResponse>> GetAllTicketsForAnEmployeeAsync(Guid employeeId);

        Task<Guid> CreateAsync(CreateTicketRequest request);
        Task UpdateAsync(Guid id, UpdateTicketRequest request);
        Task<bool> DeleteAsync(Guid id);


        Task<int> GetTicketTotalCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketInProgressCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketCompletedCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketNeedReviewCountForAnEmployeeAsync(Guid employeeId);

        Task ChangeTicketStatusAsync(Guid ticketId, TicketStatus status);
        Task ChangeTicketPriorityAsync(Guid ticketId, TicketPriority priority);
        Task SubmitForReviewAsync(Guid ticketId, TicketActionRequest request);
        Task ApproveAsync(Guid ticketId, TicketActionRequest request);
        Task RequestChangesAsync(Guid ticketId, TicketActionRequest request);
        Task ReassignAsync(Guid ticketId, TicketReassignRequest request);
        Task AddCommentAsync(Guid ticketId, TicketActionRequest request);

        Task AddAttachmentToTicketAsync(
            Guid ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes);

        Task<bool> TicketExistsAsync(Guid ticketId);
        Task<bool> EmployeeExistsAsync(Guid employeeId);
        Task<bool> ProjectExistsAsync(Guid projectId);
    }
}
