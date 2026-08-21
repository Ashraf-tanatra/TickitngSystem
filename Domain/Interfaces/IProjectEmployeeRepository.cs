using Domain.Entities;

namespace Domain.Interfaces
{
    
    public interface IProjectEmployeeRepository
    {
        IEnumerable<ProjectEmployee> GetAll();

        IEnumerable<ProjectEmployee> GetByProjectId(int projectId);

        IEnumerable<ProjectEmployee> GetByEmployeeId(int employeeId);

        ProjectEmployee? Get(int projectId, int employeeId);

        bool Exists(int projectId, int employeeId);

        void Add(ProjectEmployee projectEmployee);

        void Delete(ProjectEmployee projectEmployee);
    }
}