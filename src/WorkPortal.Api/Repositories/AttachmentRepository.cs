using Microsoft.EntityFrameworkCore;
using WorkPortal.Api.Data;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly AppDbContext db;

    public AttachmentRepository(AppDbContext db) => this.db = db;

    public async Task<List<Attachment>> GetByTask(int taskId) =>
        await db.Attachments
            .AsNoTracking()
            .Where(a => a.TaskId == taskId)
            .OrderByDescending(a => a.Id)
            .ToListAsync();

    public async Task<Attachment?> GetById(int id) =>
        await db.Attachments.FindAsync(id);

    public void Add(Attachment attachment) => db.Attachments.Add(attachment);

    public void Remove(Attachment attachment) => db.Attachments.Remove(attachment);

    public async Task SaveChanges() => await db.SaveChangesAsync();
}
