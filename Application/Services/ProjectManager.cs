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
            //if (!await _projectRepository.ProjectExistsAsync(id))
            //    throw new NullReferenceException("Project does not exist.");

            var project = await _projectRepository.GetByIdAsync(id);

            return await Task.FromResult(new ProjectResponse
            {
                Id = project!.Id,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectStatus = project.ProjectStatus.ToString(),
                ProjectManagerId = project.ProjectManagerId,
                StartDate = project.StartedAt,
                EndDate = project.EndAt,

                // hard coded
                EmployeeRole = (project.ProjectManagerId == id ? "Manager" : null)
                ?? project.ProjectEmployees?.FirstOrDefault(pe => pe.EmployeeId == id)?.Role,

                //ProjectManagerName = project.ProjectManager == null ? null
                //    : $"{project.ProjectManager.FName} {project.ProjectManager.LName}"

                //ProjectManagerName = project.ProjectManager!.FName.Aggregate(project.ProjectManager.LName, (f, l) => $"{f} {l}") == null
                //    ? null : $"{project.ProjectManager.FName} {project.ProjectManager.LName}",
            });
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
                throw new ArgumentException("Project name is required.");
            //if (!await _projectRepository.EmployeeExistsAsync(request.ProjectManagerId))
            //    throw new ArgumentException("The specified Project Manager does not exist.");

            var project = new Project
            {
                ProjectName = request.ProjectName,
                ProjectDescription = request.ProjectDescription,
                ProjectManagerId = request.ProjectManagerId, // send by the URL
                StartedAt = request.StartTime,
                EndAt = request.EndTime
            };

            await _projectRepository.CreateAsync(project);
            return project.Id;
        }
        // Need to add is deleted if it's yes then can't add him
        public async Task<bool> ProjectAddEmployeeAsync(ProjectEmployeeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            //if (!await _projectRepository.EmployeeExistsAsync(request.EmployeeId))
            //    throw new ArgumentException(
            //        "The specified employee does not exist.");

            var projectEmployee = new ProjectEmployee
            {
                EmployeeId = request.EmployeeId,
                ProjectId = request.ProjectId,
                Role = request.Role
            };
            await _projectRepository.AddEmployeeToProjectAsync(projectEmployee);
            return await Task.FromResult(true);
        }
        // need edit for testing for project manager only can edit,
        // Enhancement: to throw exeptions
        public async Task<bool> SetProjectStatusAsync(int projectId, ProjectStatus status)
        {
            //if ((int)status < 0 || (int)status > 4)
            //    throw new ArgumentException("Invalid project status.");

            await _projectRepository.SetProjectStatusAsync(projectId, status);
            return await Task.FromResult(true);
        }
        public async Task<bool> UpdateAsync(int projectId, int empId, UpdateProjectRequest request)
        {
            ArgumentException exception = new ArgumentException();

            if (request == null)
                throw exception;
            if (string.IsNullOrWhiteSpace(request.ProjectName))
                throw exception;
            if (!await _projectRepository.ProjectExistsAsync(projectId))
                throw exception;
            if (!await _projectRepository.EmployeeExistsAsync(empId))
                throw exception;
            if (!await _projectRepository.IsManagerAsync(projectId, empId))
                throw exception;

            var project = await _projectRepository.GetByIdAsync(projectId);

            project!.ProjectName = request.ProjectName;
            project.ProjectDescription = request.ProjectDescription;
            //project.ProjectManagerId = request.ProjectManagerId;
            project.StartedAt = request.StartDate;
            project.EndAt = request.EndDate;

            await _projectRepository.UpdateAsync(project);
            return await Task.FromResult(true);
        }
        public async Task<IEnumerable<EmployeeResponse>>? GetEmployeesWorkOnProjectAsync(int projectId)
        {
            var employees = await _projectRepository.GetEmployeesAsync(projectId);
            if (employees == null || employees.Count() == 0)
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
            //if (!await _projectRepository.EmployeeExistsAsync(employeeId))
            //    throw new NullReferenceException("Employee not found.");
            var project = await _projectRepository.GetAllProjectWorkedByEmployeeAsync(employeeId);
            if (project == null || project.Count() == 0)
                throw new NullReferenceException("No projects found for the specified employee.");

            return project.Select(project => new ProjectResponse
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectStatus = project.ProjectStatus.ToString(),
                ProjectManagerId = project.ProjectManagerId,
                StartDate = project.StartedAt,
                EndDate = project.EndAt,

                // hard coded
                EmployeeRole = (project.ProjectManagerId == employeeId ? "Manager" : null)
                ?? project.ProjectEmployees?.FirstOrDefault(pe => pe.EmployeeId == employeeId)?.Role,

                ProjectManagerName = project.ProjectManager == null ? null
                    : $"{project.ProjectManager.FName} {project.ProjectManager.LName}"
            });
        }
        public async Task<IEnumerable<string[]>>? GetAllProjectWorkedByEmployeeTopThreeAsync(int employeeId)
        {
            //if (!await _projectRepository.EmployeeExistsAsync(employeeId))
            //    throw new NullReferenceException("Employee not found.");
            var emp = await _projectRepository.GetAllProjectWorkedByEmployeeTopThreeAsync(employeeId);
            if (emp == null || emp.Count() == 0)
                throw new NullReferenceException("No projects found for the specified employee.");
            return emp;
        }
        public async Task<bool> DeleteAsync(int projectId, int empId) => await _projectRepository.DeleteAsync(projectId, empId);
        public async Task<bool> ProjectExistsAsync(int projectId) => await _projectRepository.ProjectExistsAsync(projectId);
        public async Task<IEnumerable<ProjectResponse>>? GetAllProjectWorkedByEmployeeWithFilterAsync(int employeeId,
            ProjectStatus FilterByStatus)
        {
            //if (!await _projectRepository.EmployeeExistsAsync(employeeId))
            //    throw new NullReferenceException("Employee not found.");

            var project = await _projectRepository.GetAllProjectWorkedByEmployeeWithFilterAsync(employeeId, FilterByStatus);
            if (project == null)
                throw new NullReferenceException("No projects found for the specified employee.");

            return project.Select(project => new ProjectResponse
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectStatus = project.ProjectStatus.ToString(),
                ProjectManagerId = project.ProjectManagerId,
                StartDate = project.StartedAt,
                EndDate = project.EndAt,

                EmployeeRole = (project.ProjectManagerId == employeeId ? "Manager" : null)
                ?? project.ProjectEmployees?.FirstOrDefault(pe => pe.EmployeeId == employeeId)?.Role
            });
        }
    }
}