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

        public async Task<bool> SetProjectStatusAsync(int projectId, ProjectStatus status)
        {
            if (!System.Enum.IsDefined(status))
                throw new ArgumentException("Invalid project status.");

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
                throw new UnauthorizedAccessException("Only the project manager can update this project.");

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
            var employees = await _projectRepository.GetEmployeesAsync(projectId);
            if (employees == null || !employees.Any())
                throw new NullReferenceException("No employees found for the specified project.");

            return employees.Select(employee => new EmployeeResponse
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
            var project = await _projectRepository.GetAllProjectWorkedByEmployeeAsync(employeeId);
            if (project == null || !project.Any())
                throw new NullReferenceException("No projects found for the specified employee.");

            return project.Select(project => MapToResponse(project, employeeId));
        }

        public async Task<IEnumerable<string[]>>? GetAllProjectWorkedByEmployeeTopThreeAsync(int employeeId)
        {
            var emp = await _projectRepository.GetAllProjectWorkedByEmployeeTopThreeAsync(employeeId);
            if (emp == null || !emp.Any())
                throw new NullReferenceException("No projects found for the specified employee.");

            return emp;
        }

        public async Task<bool> DeleteAsync(int projectId, int empId) => await _projectRepository.DeleteAsync(projectId, empId);

        public async Task<bool> ProjectExistsAsync(int projectId) => await _projectRepository.ProjectExistsAsync(projectId);

        public async Task<IEnumerable<ProjectResponse>>? GetAllProjectWorkedByEmployeeWithFilterAsync(int employeeId,
            ProjectStatus FilterByStatus)
        {
            var project = await _projectRepository.GetAllProjectWorkedByEmployeeWithFilterAsync(employeeId, FilterByStatus);
            if (project == null)
                throw new NullReferenceException("No projects found for the specified employee.");

            return project.Select(project => MapToResponse(project, employeeId));
        }

        private static ProjectResponse MapToResponse(Project project, int? employeeId = null)
        {
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
                    : $"{project.ProjectManager.FName} {project.ProjectManager.LName}"
            };
        }

        private static string? GetEmployeeRole(Project project, int employeeId)
        {
            if (project.ProjectManagerId == employeeId)
                return "Manager";

            return project.ProjectEmployees
                .FirstOrDefault(pe => pe.EmployeeId == employeeId)
                ?.Role;
        }
    }
}
