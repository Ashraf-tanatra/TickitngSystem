using ApplicationServices.DTOs.Project;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;

namespace ApplicationServices.Services
{
    public class ProjectManager : IProjectManager
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectManager(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        // Make send the manager name
        public async Task<ProjectResponse?> GetByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);

            if (project == null)
                return null;

            return MapToResponse(project);
        }

        public async Task<int> GetProjectCountAsync(int employeeId)
        {
            return await _projectRepository.GetProjectCountAsync(employeeId);
        }

        public async Task<int> CreateAsync(CreateProjectRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.ProjectName))
                throw new ArgumentException(ErrorShared.Project.ProjectNameRequired);

            if (!await _projectRepository.EmployeeExistsAsync(request.ProjectManagerId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            var project = Project.Create(
                request.ProjectName,
                request.ProjectDescription,
                request.ProjectManagerId,
                request.StartTime,
                request.EndTime);

            await _projectRepository.CreateAsync(project);
            return project.Id;
        }

        public async Task<bool> ProjectAddEmployeeAsync(ProjectEmployeeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var projectEmployee = ProjectEmployee.Create(
                request.ProjectId,
                request.EmployeeId,
                request.Role);

            await _projectRepository.AddEmployeeToProjectAsync(projectEmployee);
            return true;
        }

        public async Task<bool> RemoveEmployeeFromProjectAsync(RemoveProjectEmployeeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!await _projectRepository.ProjectExistsAsync(request.ProjectId))
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);

            if (!await _projectRepository.EmployeeExistsAsync(request.EmployeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            if (!await _projectRepository.EmployeeExistsAsync(request.ActionByEmployeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            if (!await _projectRepository.IsManagerAsync(request.ProjectId, request.ActionByEmployeeId))
                throw new UnauthorizedAccessException(ErrorShared.Project.OnlyManagerCanRemoveMembers);

            var project = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (project!.ProjectManagerId == request.EmployeeId)
                throw new InvalidOperationException(ErrorShared.Project.ProjectManagerCannotBeRemoved);

            var employeeIsAssigned = project.ProjectEmployees
                .Any(pe => pe.EmployeeId == request.EmployeeId);

            if (!employeeIsAssigned)
                throw new ArgumentException(ErrorShared.Project.EmployeeNotAssigned);

            if (await _projectRepository.EmployeeHasActiveTicketsInProjectAsync(request.ProjectId, request.EmployeeId))
                throw new InvalidOperationException(ErrorShared.Project.EmployeeHasActiveTickets);

            await _projectRepository.RemoveEmployeeFromProjectAsync(request.ProjectId, request.EmployeeId);
            return true;
        }

        public async Task<bool> SetProjectStatusAsync(int projectId, ProjectStatus status)
        {
            if (!System.Enum.IsDefined(status))
                throw new ArgumentException(ErrorShared.Project.InvalidStatus);

            await _projectRepository.SetProjectStatusAsync(projectId, status);
            return true;
        }

        public async Task<bool> UpdateAsync(int projectId, int empId, UpdateProjectRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.ProjectName))
                throw new ArgumentException(ErrorShared.Project.ProjectNameRequired);

            if (!await _projectRepository.ProjectExistsAsync(projectId))
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);

            if (!await _projectRepository.EmployeeExistsAsync(empId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            if (!await _projectRepository.IsManagerAsync(projectId, empId))
                throw new UnauthorizedAccessException(ErrorShared.Project.OnlyManagerCanUpdate);

            var project = await _projectRepository.GetByIdAsync(projectId);

            project!.UpdateDetails(
                request.ProjectName,
                request.ProjectDescription,
                request.StartDate,
                request.EndDate);

            await _projectRepository.UpdateAsync(project);
            return true;
        }

        public async Task<IEnumerable<EmployeeResponse>>? GetEmployeesWorkOnProjectAsync(int projectId)
        {
            if (!await _projectRepository.ProjectExistsAsync(projectId))
                throw new ArgumentException(ErrorShared.Project.ProjectNotFound);

            var employees = await _projectRepository.GetEmployeesAsync(projectId);

            return employees!.Select(employee => new EmployeeResponse
            {
                Id = employee.Id,
                FName = employee.FName,
                LName = employee.LName,
                Phone = employee.Phone,
                Gender = employee.Gender
            });
        }
        public async Task<IEnumerable<ProjectResponse>>? GetAllProjectWorkedByEmployeeAsync(int employeeId)
        {
            if (!await _projectRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            var project = await _projectRepository.GetAllProjectWorkedByEmployeeAsync(employeeId);
            return project!.Select(project => MapToResponse(project, employeeId));
        }

        public async Task<IEnumerable<string[]>>? GetAllProjectWorkedByEmployeeTopThreeAsync(int employeeId)
        {
            if (!await _projectRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            var emp = await _projectRepository.GetAllProjectWorkedByEmployeeTopThreeAsync(employeeId);
            return emp ?? Array.Empty<string[]>();
        }

        public async Task<IEnumerable<ProjectResponse>>? GetDashboardProjectsAsync(int employeeId)
        {
            if (!await _projectRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            var projects = await _projectRepository.GetDashboardProjectsAsync(employeeId);
            return projects!.Select(project => MapToResponse(project, employeeId));
        }

        public async Task<IEnumerable<ProjectResponse>> GetRecentActiveProjectsAsync(int employeeId)
        {
            if (!await _projectRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            var projects = await _projectRepository.GetRecentActiveProjectsAsync(employeeId);
            return projects.Select(project => MapToResponse(project, employeeId));
        }

        public async Task<IEnumerable<RecentActivityResponse>> GetRecentActivityAsync(int employeeId)
        {
            if (!await _projectRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            var projects = await _projectRepository.GetRecentActiveProjectsAsync(employeeId);

            return projects
                .Select(MapRecentActivity)
                .ToList();
        }

        public async Task<bool> DeleteAsync(int projectId, int empId) => await _projectRepository.DeleteAsync(projectId, empId);

        public async Task<bool> ProjectExistsAsync(int projectId) => await _projectRepository.ProjectExistsAsync(projectId);

        public async Task<IEnumerable<ProjectResponse>>? GetAllProjectWorkedByEmployeeWithFilterAsync(int employeeId,
            ProjectStatus FilterByStatus)
        {
            if (!await _projectRepository.EmployeeExistsAsync(employeeId))
                throw new ArgumentException(ErrorShared.Project.EmployeeNotFound);

            if (!System.Enum.IsDefined(FilterByStatus))
                throw new ArgumentException(ErrorShared.Project.InvalidStatus);

            var project = await _projectRepository.GetAllProjectWorkedByEmployeeWithFilterAsync(employeeId, FilterByStatus);
            return project!.Select(project => MapToResponse(project, employeeId));
        }

        private static ProjectResponse MapToResponse(Project project, int? employeeId = null)
        {
            var ticketCount = project.ProjectTickets.Count;
            var doneTicketCount = project.ProjectTickets.Count(ticket =>
                ticket.TicketStatus == TicketStatus.Done ||
                ticket.TicketStatus == TicketStatus.Completed);

            return new ProjectResponse
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectStatus = project.ProjectStatus.ToString(),
                ProjectManagerId = project.ProjectManagerId,
                StartDate = project.StartedAt,
                EndDate = project.EndAt,
                EmployeeRole = employeeId.HasValue
                    ? GetEmployeeRole(project, employeeId.Value)
                    : null,
                ProjectManagerName = project.ProjectManager == null
                    ? null
                    : $"{project.ProjectManager.FName} {project.ProjectManager.LName}",
                EmployeeCount = project.ProjectEmployees.Count(projectEmployee =>
                    !projectEmployee.Employee.IsDeleted) + 1,
                TicketCount = ticketCount,
                InProgressTicketCount = project.ProjectTickets.Count(ticket =>
                    ticket.TicketStatus == TicketStatus.InProgress),
                NeedReviewTicketCount = project.ProjectTickets.Count(ticket =>
                    ticket.TicketStatus == TicketStatus.NeedReview ||
                    ticket.TicketStatus == TicketStatus.InReview),
                DoneTicketCount = doneTicketCount,
                ProgressPercentage = ticketCount == 0
                    ? 0
                    : (int)Math.Round(doneTicketCount * 100.0 / ticketCount)
            };
        }

        private static RecentActivityResponse MapRecentActivity(Project project)
        {
            var activity = GetLatestActivity(project);

            return new RecentActivityResponse
            {
                ProjectId = project.Id,
                TicketId = activity.TicketId,
                ProjectName = project.ProjectName,
                Activity = activity.Description,
                OccurredAt = activity.OccurredAt,
                TimeAgo = FormatTimeAgo(activity.OccurredAt)
            };
        }

        private static ActivityCandidate GetLatestActivity(Project project)
        {
            var specificActivities = new List<ActivityCandidate>();

            foreach (var ticket in project.ProjectTickets)
            {
                specificActivities.Add(new ActivityCandidate(
                    ErrorShared.RecentActivity.TicketCreated(ticket.TicketTitle),
                    ticket.CreatedAt,
                    ticket.TicketId));

                if (ticket.UpdatedAt is DateTime ticketUpdatedAt &&
                    ticketUpdatedAt > ticket.CreatedAt.AddSeconds(1))
                {
                    specificActivities.Add(new ActivityCandidate(
                        ErrorShared.RecentActivity.TicketUpdated(ticket.TicketTitle),
                        ticketUpdatedAt,
                        ticket.TicketId));
                }

                foreach (var history in ticket.TicketHistories)
                {
                    specificActivities.Add(new ActivityCandidate(
                        FormatHistoryActivity(ticket.TicketTitle, history),
                        ToLocalTime(history.ModifiedAt),
                        ticket.TicketId));
                }

                foreach (var attachment in ticket.AttachmentURL)
                {
                    specificActivities.Add(new ActivityCandidate(
                        ErrorShared.RecentActivity.AttachmentAdded(attachment.OriginalFileName, ticket.TicketTitle),
                        ToLocalTime(attachment.CreatedAt),
                        ticket.TicketId));
                }
            }

            var candidates = new List<ActivityCandidate>
            {
                new ActivityCandidate(
                    ErrorShared.RecentActivity.ProjectCreated,
                    ToLocalTime(project.CreatedAt),
                    null)
            };

            if (project.UpdatedAt is DateTime updatedAt)
            {
                var localUpdatedAt = ToLocalTime(updatedAt);
                var isRelatedToSpecificActivity = specificActivities.Any(activity =>
                    Math.Abs((activity.OccurredAt - localUpdatedAt).TotalSeconds) <= 2);

                if (!isRelatedToSpecificActivity)
                {
                    candidates.Add(new ActivityCandidate(
                        ErrorShared.RecentActivity.ProjectUpdated,
                        localUpdatedAt,
                        null));
                }
            }

            candidates.AddRange(specificActivities);
            return candidates
                .OrderByDescending(activity => activity.OccurredAt)
                .First();
        }

        private static string FormatHistoryActivity(string ticketTitle, TicketHistory history)
        {
            return history.Action switch
            {
                ErrorShared.Ticket.SubmitForReviewAction =>
                        ErrorShared.RecentActivity.TicketMovedTo(
                        ticketTitle,
                        FormatDisplayValue(history.NewValue ?? nameof(TicketStatus.NeedReview))),
                ErrorShared.Ticket.ApproveAction =>
                    ErrorShared.RecentActivity.TicketMarkedCompleted(ticketTitle),
                ErrorShared.Ticket.RequestChangesAction =>
                    ErrorShared.RecentActivity.TicketMovedTo(
                        ticketTitle,
                        FormatDisplayValue(history.NewValue ?? nameof(TicketStatus.InProgress))),
                ErrorShared.Ticket.ReassignAction =>
                    ErrorShared.RecentActivity.TicketReassigned(ticketTitle),
                ErrorShared.Ticket.CommentAction =>
                    ErrorShared.RecentActivity.TicketCommented(ticketTitle),
                ErrorShared.Ticket.UnassignedDueToAccountDeactivationAction =>
                    ErrorShared.RecentActivity.TicketUnassigned(ticketTitle),
                _ when !string.IsNullOrWhiteSpace(history.NewValue) =>
                    ErrorShared.RecentActivity.TicketChangedTo(ticketTitle, FormatDisplayValue(history.NewValue)),
                _ => ErrorShared.RecentActivity.TicketUpdated(ticketTitle)
            };
        }

        private static string FormatTimeAgo(DateTime occurredAt)
        {
            var elapsed = DateTime.Now - occurredAt;

            if (elapsed.TotalMinutes < 1)
                return ErrorShared.RecentActivity.JustNow;

            if (elapsed.TotalHours < 1)
                return ErrorShared.RecentActivity.MinutesAgo((long)elapsed.TotalMinutes);

            if (elapsed.TotalHours < 24)
                return ErrorShared.RecentActivity.HoursAgo((long)elapsed.TotalHours);

            if (occurredAt.Date == DateTime.Today.AddDays(-1))
                return ErrorShared.RecentActivity.Yesterday;

            return ErrorShared.RecentActivity.DaysAgo(Math.Max(2, (long)elapsed.TotalDays));
        }

        private static DateTime ToLocalTime(DateTime value) =>
            value.Kind == DateTimeKind.Utc ? value.ToLocalTime() : value;

        private static string FormatDisplayValue(string value)
        {
            var result = new System.Text.StringBuilder(value.Length + 5);

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (index > 0 && char.IsUpper(character) && char.IsLower(value[index - 1]))
                    result.Append(' ');

                result.Append(character);
            }

            return result.ToString();
        }

        private sealed record ActivityCandidate(string Description, DateTime OccurredAt, int? TicketId);

        private static string? GetEmployeeRole(Project project, int employeeId)
        {
            if (project.ProjectManagerId == employeeId)
                return ErrorShared.Project.ManagerRole;

            return project.ProjectEmployees
                .FirstOrDefault(pe => pe.EmployeeId == employeeId)
                ?.Role;
        }
    }
}
