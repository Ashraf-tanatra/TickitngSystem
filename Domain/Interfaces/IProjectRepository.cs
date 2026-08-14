using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        IEnumerable<Project> GetAll();

        Project? GetById(int id);

        void Add(Project project);

        void Update(Project project);

        void Delete(Project project);
    }
}