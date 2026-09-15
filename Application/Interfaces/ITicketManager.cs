using ApplicationServices.DTOs.Ticket;
using Domain.Enum;

namespace ApplicationServices.Interfaces
{
    public interface ITicketManager
    {
        Task<TicketResponse?> GetByIdAsync(Guid id);
        Task<IEnumerable<TicketHistoryResponse>> GetTicketHistoryAsync(Guid ticketId);
        Task<IEnumerable<TicketAttachmentResponse>> GetTicketAttachmentsAsync(Guid ticketId);
        Task<Guid?> GetAttachmentTicketIdAsync(string storedFileName);

        Task<IEnumerable<TicketResponse>> GetAllTicketsForAProjectAsync(Guid projectId);
        Task<IEnumerable<TicketResponse>> GetAllTicketsForAnEmployeeAsync(Guid employeeId);

        Task<Guid> CreateAsync(CreateTicketRequest request, Guid actionByEmployeeId);
        Task UpdateAsync(Guid id, UpdateTicketRequest request, Guid actionByEmployeeId);
        Task<bool> DeleteAsync(Guid id, Guid actionByEmployeeId);


        Task<int> GetTicketTotalCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketInProgressCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketCompletedCountForAnEmployeeAsync(Guid employeeId);
        Task<int> GetTicketNeedReviewCountForAnEmployeeAsync(Guid employeeId);

        Task ChangeTicketStatusAsync(Guid ticketId, TicketStatus status, Guid actionByEmployeeId);
        Task ChangeTicketPriorityAsync(Guid ticketId, TicketPriority priority, Guid actionByEmployeeId);
        Task SubmitForReviewAsync(Guid ticketId, TicketActionRequest request, Guid actionByEmployeeId);
        Task ApproveAsync(Guid ticketId, TicketActionRequest request, Guid actionByEmployeeId);
        Task RequestChangesAsync(Guid ticketId, TicketActionRequest request, Guid actionByEmployeeId);
        Task ReassignAsync(Guid ticketId, TicketReassignRequest request, Guid actionByEmployeeId);
        Task AddCommentAsync(Guid ticketId, TicketActionRequest request, Guid actionByEmployeeId);

        Task AddAttachmentToTicketAsync(
            Guid ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes,
            Guid actionByEmployeeId);

        Task<bool> TicketExistsAsync(Guid ticketId);
        Task<bool> EmployeeExistsAsync(Guid employeeId);
        Task<bool> ProjectExistsAsync(Guid projectId);
    }
}
