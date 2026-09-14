using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public interface ITaskRepository
{
    Task<(List<WorkTask> Items, int Total)> Search(TaskQuery query);
    Task<WorkTask?> GetById(int id);
    Task<WorkTask?> GetForUpdate(int id);
    Task<bool> Exists(int id);
    void Add(WorkTask task);
    void Remove(WorkTask task);

    Task<List<Comment>> GetComments(int taskId);
    Task<Comment?> GetComment(int taskId, int commentId);
    void AddComment(Comment comment);
    void RemoveComment(Comment comment);

    Task SaveChanges();
}
