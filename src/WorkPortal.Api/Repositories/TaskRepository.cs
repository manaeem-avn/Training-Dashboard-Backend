using Microsoft.EntityFrameworkCore;
using WorkPortal.Api.Data;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext db;

    public TaskRepository(AppDbContext db) => this.db = db;

    public async Task<(List<WorkTask> Items, int Total)> Search(TaskQuery query)
    {
        IQueryable<WorkTask> source = db.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Comments)
            .Include(t => t.Attachments);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            source = source.Where(t => t.Title.Contains(term) || t.Description.Contains(term));
        }

        if (query.ProjectId.HasValue) source = source.Where(t => t.ProjectId == query.ProjectId.Value);
        if (query.AssigneeId.HasValue) source = source.Where(t => t.AssigneeId == query.AssigneeId.Value);
        if (!string.IsNullOrWhiteSpace(query.Status)) source = source.Where(t => t.Status == query.Status);
        if (!string.IsNullOrWhiteSpace(query.Priority)) source = source.Where(t => t.Priority == query.Priority);

        source = query.SortBy.ToLower() switch
        {
            "title" => query.Desc ? source.OrderByDescending(t => t.Title) : source.OrderBy(t => t.Title),
            "status" => query.Desc ? source.OrderByDescending(t => t.Status) : source.OrderBy(t => t.Status),
            "priority" => query.Desc ? source.OrderByDescending(t => t.Priority) : source.OrderBy(t => t.Priority),
            "duedate" => query.Desc ? source.OrderByDescending(t => t.DueDate) : source.OrderBy(t => t.DueDate),
            _ => query.Desc ? source.OrderByDescending(t => t.Id) : source.OrderBy(t => t.Id)
        };

        int total = await source.CountAsync();

        List<WorkTask> items = await source
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<WorkTask?> GetById(int id) =>
        await db.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<WorkTask?> GetForUpdate(int id) =>
        await db.Tasks.FindAsync(id);

    public async Task<bool> Exists(int id) =>
        await db.Tasks.AnyAsync(t => t.Id == id);

    public void Add(WorkTask task) => db.Tasks.Add(task);

    public void Remove(WorkTask task) => db.Tasks.Remove(task);

    public async Task<List<Comment>> GetComments(int taskId) =>
        await db.Comments
            .AsNoTracking()
            .Include(c => c.Author)
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.Id)
            .ToListAsync();

    public async Task<Comment?> GetComment(int taskId, int commentId) =>
        await db.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.TaskId == taskId);

    public void AddComment(Comment comment) => db.Comments.Add(comment);

    public void RemoveComment(Comment comment) => db.Comments.Remove(comment);

    public async Task SaveChanges() => await db.SaveChangesAsync();
}
