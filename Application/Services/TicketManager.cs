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

        public async Task<TicketResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket == null)
                return null;

            return MapToResponse(ticket);
        }

        public async Task<IEnumerable<TicketHistoryResponse>> GetTicketHistoryAsync(int ticketId)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            var history = await _ticketRepository.GetTicketHistoryAsync(ticketId);
            return history.Select(MapHistoryToResponse);
        }

        public async Task<IEnumerable<TicketAttachmentResponse>> GetTicketAttachmentsAsync(int ticketId)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            var attachments = await _ticketRepository.GetTicketAttachmentsAsync(ticketId);
            return attachments.Select(MapAttachmentToResponse);
        }

        public async Task<IEnumerable<TicketResponse>> GetAllTicketsForAProjectAsync(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            if (!await _ticketRepository.ProjectExistsAsync(projectId))
                throw new ArgumentException(ErrorShared.Ticket.ProjectNotFound);

            var tickets = await _ticketRepository.GetAllTicketsForAProjectAsync(projectId);
            if (!tickets.Any())
                throw new ArgumentException(ErrorShared.Ticket.NoTicketsForProject);

            return tickets.Select(MapToResponse);
        }

        public async Task<IEnumerable<TicketResponse>> GetAllTicketsForAnEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            var tickets = await _ticketRepository.GetAllTicketsForAnEmployeeAsync(employeeId);
            if (!tickets.Any())
                throw new ArgumentException(ErrorShared.Ticket.NoTicketsForEmployee);

            return tickets.Select(MapToResponse);
        }

        public async Task<int> CreateAsync(CreateTicketRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.TicketTitle))
                throw new ArgumentException(ErrorShared.Ticket.TicketTitleRequired);

            if (!await _ticketRepository.EmployeeExistsAsync(request.EmployeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.EmployeeExistsAsync(request.TicketCreatedById))
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
                request.TicketCreatedById);

            if (!await _ticketRepository.IsManagerAsync(ticket.TicketCreatedById, ticket.ProjectId))
                throw new UnauthorizedAccessException(ErrorShared.Ticket.UnauthorizedTicketCreation);

            await _ticketRepository.CreateAsync(ticket);
            return ticket.TicketId;

        }

        public async Task UpdateAsync(int id, UpdateTicketRequest request)
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

            if (!await _ticketRepository.IsEmployeeAssignedToProjectAsync(request.EmployeeId, ticket!.ProjectId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotAssignedToProject);

            ticket.UpdateDetails(
                request.TicketTitle,
                request.DueTo,
                request.Description,
                request.EmployeeId);

            await _ticketRepository.UpdateAsync(ticket);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (!await _ticketRepository.TicketExistsAsync(id))
                return false;

            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket is null)
            {
                return false;
            }

            ticket.ChangeStatus(TicketStatus.Cancelled);
            await _ticketRepository.UpdateAsync(ticket);
            return true;
        }

        public async Task<int> GetTicketTotalCountForAnEmployeeAsync(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            return await _ticketRepository.GetTicketTotalCountForAnEmployeeAsync(employeeId);
        }

        public async Task<int> GetTicketInProgressCountForAnEmployeeAsync(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            return await _ticketRepository.GetTicketInProgressCountForAnEmployeeAsync(employeeId);
        }

        public async Task<int> GetTicketCompletedCountForAnEmployeeAsync(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            return await _ticketRepository.GetTicketCompletedCountForAnEmployeeAsync(employeeId);
        }

        public async Task<int> GetTicketNeedReviewCountForAnEmployeeAsync(int employeeId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            return await _ticketRepository.GetTicketNeedReviewCountForAnEmployeeAsync(employeeId);
        }

        public async Task ChangeTicketStatusAsync(int ticketId, TicketStatus status)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            await _ticketRepository.ChangeTicketStatusAsync(ticketId, status);
        }

        public async Task ChangeTicketPriorityAsync(int ticketId, TicketPriority priority)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            await _ticketRepository.ChangeTicketPriorityAsync(ticketId, priority);
        }

        public async Task SubmitForReviewAsync(int ticketId, TicketActionRequest request)
        {
            ValidateActionRequest(request);

            var ticket = await GetTicketOrThrowAsync(ticketId);

            if (ticket.EmployeeId != request.ActionByEmployeeId)
                throw new UnauthorizedAccessException(ErrorShared.Ticket.OnlyAssigneeCanSubmitForReview);

            var oldStatus = ticket.TicketStatus.ToString();
            ticket.SubmitForReview();

            var history = TicketHistory.Create(
                ticket.TicketId,
                request.ActionByEmployeeId,
                "SubmitForReview",
                oldStatus,
                ticket.TicketStatus.ToString(),
                note: request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task ApproveAsync(int ticketId, TicketActionRequest request)
        {
            ValidateActionRequest(request);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureManagerAsync(request.ActionByEmployeeId, ticket.ProjectId);

            var oldStatus = ticket.TicketStatus.ToString();
            ticket.Approve();

            var history = TicketHistory.Create(
                ticket.TicketId,
                request.ActionByEmployeeId,
                "Approve",
                oldStatus,
                ticket.TicketStatus.ToString(),
                note: request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task RequestChangesAsync(int ticketId, TicketActionRequest request)
        {
            ValidateActionRequest(request);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureManagerAsync(request.ActionByEmployeeId, ticket.ProjectId);

            var oldStatus = ticket.TicketStatus.ToString();
            ticket.RequestChanges();

            var history = TicketHistory.Create(
                ticket.TicketId,
                request.ActionByEmployeeId,
                "RequestChanges",
                oldStatus,
                ticket.TicketStatus.ToString(),
                note: request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task ReassignAsync(int ticketId, TicketReassignRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.ActionByEmployeeId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (request.ToEmployeeId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureManagerAsync(request.ActionByEmployeeId, ticket.ProjectId);

            if (!await _ticketRepository.EmployeeExistsAsync(request.ToEmployeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.IsEmployeeAssignedToProjectAsync(request.ToEmployeeId, ticket.ProjectId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotAssignedToProject);

            var fromEmployeeId = ticket.EmployeeId;
            ticket.Reassign(request.ToEmployeeId);

            var history = TicketHistory.Create(
                ticket.TicketId,
                request.ActionByEmployeeId,
                "Reassign",
                fromEmployeeId.ToString(),
                request.ToEmployeeId.ToString(),
                fromEmployeeId,
                request.ToEmployeeId,
                request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task AddCommentAsync(int ticketId, TicketActionRequest request)
        {
            ValidateActionRequest(request);

            if (string.IsNullOrWhiteSpace(request.Note))
                throw new ArgumentException(ErrorShared.Ticket.CommentRequired);

            var ticket = await GetTicketOrThrowAsync(ticketId);
            await EnsureProjectMemberAsync(request.ActionByEmployeeId, ticket.ProjectId);

            var history = TicketHistory.Create(
                ticket.TicketId,
                request.ActionByEmployeeId,
                "Comment",
                note: request.Note);

            await _ticketRepository.UpdateWithHistoryAsync(ticket, history);
        }

        public async Task AddAttachmentToTicketAsync(
            int ticketId,
            string url,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeInBytes)
        {
            if (!await _ticketRepository.TicketExistsAsync(ticketId))
                throw new KeyNotFoundException(ErrorShared.Ticket.TicketNotFound);

            await _ticketRepository.AddAttachmentToTicketAsync(
                ticketId,
                url,
                originalFileName,
                storedFileName,
                contentType,
                sizeInBytes);
        }

        public async Task<bool> TicketExistsAsync(int ticketId)
        {
            return await _ticketRepository.TicketExistsAsync(ticketId);
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _ticketRepository.EmployeeExistsAsync(employeeId);
        }

        public async Task<bool> ProjectExistsAsync(int projectId)
        {
            return await _ticketRepository.ProjectExistsAsync(projectId);
        }

        private async Task<Ticket> GetTicketOrThrowAsync(int ticketId)
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

            if (request.ActionByEmployeeId <= 0)
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);
        }

        private async Task EnsureManagerAsync(int employeeId, int projectId)
        {
            if (!await _ticketRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Ticket.EmployeeNotFound);

            if (!await _ticketRepository.IsManagerAsync(employeeId, projectId))
                throw new UnauthorizedAccessException(ErrorShared.Ticket.UnauthorizedTicketReview);
        }

        private async Task EnsureProjectMemberAsync(int employeeId, int projectId)
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
                Attachments = ticket.AttachmentURL.Select(MapAttachmentToResponse)
            };
        }

        private static TicketHistoryResponse MapHistoryToResponse(TicketHistory history)
        {
            return new TicketHistoryResponse
            {
                Id = history.Id,
                TicketId = history.TicketId,
                Action = history.Action,
                OldValue = history.OldValue,
                NewValue = history.NewValue,
                Note = history.Note,
                ActionByEmployeeId = history.ActionByEmployeeId,
                ActionByEmployeeName = FormatEmployeeName(history.ActionByEmployee),
                FromEmployeeId = history.FromEmployeeId,
                FromEmployeeName = FormatEmployeeName(history.FromEmployee),
                ToEmployeeId = history.ToEmployeeId,
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
                Id = attachment.Id,
                TicketId = attachment.TicketId,
                OriginalFileName = attachment.OriginalFileName,
                StoredFileName = attachment.StoredFileName,
                ContentType = attachment.ContentType,
                SizeInBytes = attachment.SizeInBytes,
                Url = attachment.URL
            };
        }
    }

}
