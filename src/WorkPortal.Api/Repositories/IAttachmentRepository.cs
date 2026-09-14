using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public interface IAttachmentRepository
{
    Task<List<Attachment>> GetByTask(int taskId);
    Task<Attachment?> GetById(int id);
    void Add(Attachment attachment);
    void Remove(Attachment attachment);
    Task SaveChanges();
}
