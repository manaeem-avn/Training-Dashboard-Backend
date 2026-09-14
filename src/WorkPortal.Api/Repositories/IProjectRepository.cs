using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public interface IProjectRepository
{
    Task<(List<Project> Items, int Total)> Search(ProjectQuery query);
    Task<Project?> GetById(int id);
    Task<Project?> GetForUpdate(int id);
    Task<bool> AnyOwnedBy(int userId);
    void Add(Project project);
    void Remove(Project project);
    Task SaveChanges();
}
