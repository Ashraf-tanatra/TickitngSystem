using ApplicationServices.DTOs.Ticket;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class TicketManager : ITicketManager
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketManager(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<TicketResponse?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket == null)
                return null;

            return MapToResponse(ticket);
        }

        public async Task<IEnumerable<TicketHistoryResponse>> GetTicketHistoryAsync(Guid ticketId)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            var history = await _ticketRepository.GetTicketHistoryAsync(ticketId);
            return history.Select(MapHistoryToResponse);
        }

        public async Task<IEnumerable<TicketAttachmentResponse>> GetTicketAttachmentsAsync(Guid ticketId)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            var attachments = await _ticketRepository.GetTicketAttachmentsAsync(ticketId);
            return attachments.Select(MapAttachmentToResponse);
        }

        public async Task<Guid?> GetAttachmentTicketIdAsync(string storedFileName)
        {
            if (string.IsNullOrWhiteSpace(storedFileName))
                return null;

            var attachment = await _ticketRepository
                .GetAttachmentByStoredFileNameAsync(Path.GetFileName(storedFileName));

            return attachment?.TicketId;
        }

        public async Task<IEnumerable<TicketResponse>> GetAllTicketsForAProjectAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            if (!await _ticketRepository.ProjectExistsAsync(projectId))
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            var tickets = await _ticketRepository.GetAllTicketsForAProjectAsync(projectId);
            return tickets.Select(MapToResponse);
        }

        public async Task<IEnumerable<TicketResponse>> GetAllTicketsForAnEmployeeAsync(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            var tickets = await _ticketRepository.GetAllTicketsForAnEmployeeAsync(employeeId);
            return tickets.Select(MapToResponse);
        }

        public async Task<Guid> CreateAsync(CreateTicketRequest request, Guid actionByEmployeeId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException(ErrorShared.Ticket.TicketTitleRequired);

            if (!await _ticketRepository.EmployeeExistsAsync(request.EmployeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.EmployeeExistsAsync(actionByEmployeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.ProjectExistsAsync(request.ProjectId))
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            if (!await _ticketRepository.IsEmployeeAssignedToProjectAsync(request.EmployeeId, request.ProjectId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotAssignedToProject);

            var ticket = Ticket.Create(
                request.TicketTitle,
                request.DueTo,
                request.Description,
                request.Priority,
                request.ProjectId,
                request.EmployeeId,
                actionByEmployeeId);

            if (!await _ticketRepository.IsManagerAsync(ticket.TicketCreatedById, ticket.ProjectId))
                throw new UnauthorizedAccessException(ErrorShared.Ticket.UnauthorizedTicketCreation);

            await _ticketRepository.CreateAsync(ticket);
            return ticket.TicketId;

        }

        public async Task UpdateAsync(Guid id, UpdateTicketRequest request, Guid actionByEmployeeId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!await _ticketRepository.TicketExistsAsync(id))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException(ErrorShared.Ticket.TicketTitleRequired);

            if (!await _ticketRepository.EmployeeExistsAsync(request.EmployeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            var ticket = await _ticketRepository.GetByIdAsync(id);

            await EnsureManagerAsync(actionByEmployeeId, ticket!.ProjectId);

            if (!await _ticketRepository.IsEmployeeAssignedToProjectAsync(request.EmployeeId, ticket.ProjectId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotAssignedToProject);

            ticket.UpdateDetails(
                request.TicketTitle,
                request.DueTo,
                request.Description,
                request.EmployeeId);

            await _ticketRepository.UpdateAsync(ticket);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid actionByEmployeeId)
        {
            if (!await _ticketRepository.TicketExistsAsync(id))
                return false;

            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket is null)
            {
                return false;
            }

            await EnsureManagerAsync(actionByEmployeeId, ticket.ProjectId);

            ticket.ChangeStatus(TicketStatus.Cancelled);
            await _ticketRepository.UpdateAsync(ticket);
            return true;
        }

        public async Task<TicketCountsResponse> GetTicketCountsForAnEmployeeAsync(Guid employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            var counts = await _ticketRepository.GetTicketCountsForAnEmployeeAsync(employeeId);
            return new TicketCountsResponse
            {
                TicketCount = counts.TicketCount,
                InProgressCount = counts.InProgressCount,
                NeedReviewCount = counts.NeedReviewCount
            };
        }

        public async Task<int> GetTicketCompletedCountForAnEmployeeAsync(Guid employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            return await _ticketRepository.GetTicketCompletedCountForAnEmployeeAsync(employeeId);
        }

        public async Task ChangeTicketStatusAsync(Guid ticketId, TicketStatus status, Guid actionByEmployeeId)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureManagerAsync(actionByEmployeeId, ticket.ProjectId);
            await _ticketRepository.ChangeTicketStatusAsync(ticketId, status);
        }

        public async Task ChangeTicketPriorityAsync(Guid ticketId, TicketPriority priority, Guid actionByEmployeeId)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureManagerAsync(actionByEmployeeId, ticket.ProjectId);
            await _ticketRepository.ChangeTicketPriorityAsync(ticketId, priority);
        }

        public async Task SubmitForReviewAsync(Guid ticketId, TicketActionRequest request, Guid actionByEmployeeId)
        {
            ValidateActionRequest(request);

            var ticket = await GetTicketOrThrowAsync(ticketId);

            if (ticket.EmployeeId != actionByEmployeeId)
                throw new UnauthorizedAccessException(ErrorShared.Ticket.OnlyAssigneeCanSubmitForReview);

            var oldStatus = ticket.TicketStatus.ToString();
            ticket.SubmitForReview();

            var history = TicketHistory.Create(
                ticket.TicketId,
                actionByEmployeeId,
                ErrorShared.Ticket.SubmitForReviewAction,
                oldStatus,
                ticket.TicketStatus.ToString(),
                note: request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task ApproveAsync(Guid ticketId, TicketActionRequest request, Guid actionByEmployeeId)
        {
            ValidateActionRequest(request);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureManagerAsync(actionByEmployeeId, ticket.ProjectId);

            var oldStatus = ticket.TicketStatus.ToString();
            ticket.Approve();

            var history = TicketHistory.Create(
                ticket.TicketId,
                actionByEmployeeId,
                ErrorShared.Ticket.ApproveAction,
                oldStatus,
                ticket.TicketStatus.ToString(),
                note: request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task RequestChangesAsync(Guid ticketId, TicketActionRequest request, Guid actionByEmployeeId)
        {
            ValidateActionRequest(request);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureManagerAsync(actionByEmployeeId, ticket.ProjectId);

            var oldStatus = ticket.TicketStatus.ToString();
            ticket.RequestChanges();

            var history = TicketHistory.Create(
                ticket.TicketId,
                actionByEmployeeId,
                ErrorShared.Ticket.RequestChangesAction,
                oldStatus,
                ticket.TicketStatus.ToString(),
                note: request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task ReassignAsync(Guid ticketId, TicketReassignRequest request, Guid actionByEmployeeId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (actionByEmployeeId == Guid.Empty)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (request.ToEmployeeId == Guid.Empty)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureManagerAsync(actionByEmployeeId, ticket.ProjectId);

            if (!await _ticketRepository.EmployeeExistsAsync(request.ToEmployeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.IsEmployeeAssignedToProjectAsync(request.ToEmployeeId, ticket.ProjectId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotAssignedToProject);

            var fromEmployeeId = ticket.EmployeeId;
            ticket.Reassign(request.ToEmployeeId);

            var history = TicketHistory.Create(
                ticket.TicketId,
                actionByEmployeeId,
                ErrorShared.Ticket.ReassignAction,
                fromEmployeeId.ToString(),
                request.ToEmployeeId.ToString(),
                fromEmployeeId,
                request.ToEmployeeId,
                request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task AddCommentAsync(Guid ticketId, TicketActionRequest request, Guid actionByEmployeeId)
        {
            ValidateActionRequest(request);

            if (string.IsNullOrWhiteSpace(request.Note))
                throw new ArgumentException(ErrorShared.Ticket.CommentRequired);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureProjectMemberAsync(actionByEmployeeId, ticket.ProjectId);

            var history = TicketHistory.Create(
                ticket.TicketId,
                actionByEmployeeId,
                ErrorShared.Ticket.CommentAction,
                note: request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task AddAttachmentToTicketAsync(
            Guid ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes,
            Guid actionByEmployeeId)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureProjectMemberAsync(actionByEmployeeId, ticket.ProjectId);

            await _ticketRepository.AddAttachmentToTicketAsync(
                ticketId,
                url,
                originalFileName,
                storedFileName,
                contentType,
                sizeInBytes);
        }

        public async Task<bool> TicketExistsAsync(Guid ticketId)
        {
            return await _ticketRepository.TicketExistsAsync(ticketId);
        }

        public async Task<bool> EmployeeExistsAsync(Guid employeeId)
        {
            return await _ticketRepository.EmployeeExistsAsync(employeeId);
        }

        public async Task<bool> ProjectExistsAsync(Guid projectId)
        {
            return await _ticketRepository.ProjectExistsAsync(projectId);
        }

        private async Task<Ticket> GetTicketOrThrowAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket is null)
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            return ticket;
        }

        private static void ValidateActionRequest(TicketActionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

        }

        private async Task EnsureManagerAsync(Guid employeeId, Guid projectId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.IsManagerAsync(employeeId, projectId))
                throw new UnauthorizedAccessException(ErrorShared.Ticket.UnauthorizedTicketReview);
        }

        private async Task EnsureProjectMemberAsync(Guid employeeId, Guid projectId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (await _ticketRepository.IsManagerAsync(employeeId, projectId))
                return;

            if (!await _ticketRepository.IsEmployeeAssignedToProjectAsync(employeeId, projectId))
                throw new UnauthorizedAccessException(ErrorShared.Ticket.UnauthorizedTicketComment);
        }

        private static TicketResponse MapToResponse(Ticket ticket)
        {
            return new TicketResponse
            {
                TicketId = ticket.TicketId,
                TicketTitle = ticket.TicketTitle,
                DueTo = ticket.DueTo,
                TicketStatus = ticket.TicketStatus.ToString(),
                Priority = ticket.Priority.ToString(),
                Description = ticket.Description,
                EmployeeId = ticket.EmployeeId,
                EmployeeName = ticket.Employee == null
                    ? null
                    : $"{ticket.Employee.FName} {ticket.Employee.LName}",
                ProjectId = ticket.ProjectId,
                ProjectName = ticket.Project?.ProjectName,
                Attachments = ticket.AttachmentURL.Select(MapAttachmentToResponse).ToArray()
            };
        }

        private static TicketHistoryResponse MapHistoryToResponse(TicketHistory history)
        {
            return new TicketHistoryResponse
            {
                Action = history.Action,
                OldValue = history.OldValue,
                NewValue = history.NewValue,
                Note = history.Note,
                ActionByEmployeeName = FormatEmployeeName(history.ActionByEmployee),
                FromEmployeeName = FormatEmployeeName(history.FromEmployee),
                ToEmployeeName = FormatEmployeeName(history.ToEmployee),
                ModifiedAt = history.ModifiedAt
            };
        }

        private static string? FormatEmployeeName(Employee? employee)
        {
            if (employee == null)
                return null;

            return $"{employee.FName} {employee.LName}";
        }

        private static TicketAttachmentResponse MapAttachmentToResponse(TicketAttachments attachment)
        {
            return new TicketAttachmentResponse
            {
                OriginalFileName = attachment.OriginalFileName,
                ContentType = attachment.ContentType,
                SizeInBytes = attachment.SizeInBytes,
                Url = attachment.URL
            };
        }
    }

}
