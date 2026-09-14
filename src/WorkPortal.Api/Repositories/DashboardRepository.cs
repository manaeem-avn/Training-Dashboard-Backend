using Microsoft.EntityFrameworkCore;
using WorkPortal.Api.Data;

namespace WorkPortal.Api.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext db;

    public DashboardRepository(AppDbContext db) => this.db = db;

    public async Task<int> CountProjects(string? status = null) =>
        status is null
            ? await db.Projects.CountAsync()
            : await db.Projects.CountAsync(p => p.Status == status);

    public async Task<int> CountTasks() => await db.Tasks.CountAsync();

    public async Task<int> CountOpenTasks() => await db.Tasks.CountAsync(t => t.Status != "Done");

    public async Task<int> CountOverdueTasks(DateTime today) =>
        await db.Tasks.CountAsync(t => t.Status != "Done" && t.DueDate != null && t.DueDate < today);

    public async Task<int> CountUsers() => await db.Users.CountAsync();

    public async Task<List<CountByLabel>> TasksByStatus() =>
        await db.Tasks.GroupBy(t => t.Status)
            .Select(g => new CountByLabel(g.Key, g.Count()))
            .ToListAsync();

    public async Task<List<CountByLabel>> TasksByPriority() =>
        await db.Tasks.GroupBy(t => t.Priority)
            .Select(g => new CountByLabel(g.Key, g.Count()))
            .ToListAsync();

    public async Task<List<CountByLabel>> TasksPerProject() =>
        await db.Tasks.GroupBy(t => t.Project.Name)
            .Select(g => new CountByLabel(g.Key, g.Count()))
            .ToListAsync();
}
