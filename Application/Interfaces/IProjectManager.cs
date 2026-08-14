using Domain.Entities;

namespace Domain.EntityManager
{
    public interface IProjectManager
    {
        public void CreateNewProject(Project project);
        public void DeleteProject(int projectId);
        public void EditProject(Project project);
        public IEnumerable<Project> Projects();
        public Project GetProjectByID(int Id);


    }
}
