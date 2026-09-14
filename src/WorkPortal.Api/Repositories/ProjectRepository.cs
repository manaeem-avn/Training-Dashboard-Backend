using Microsoft.EntityFrameworkCore;
using WorkPortal.Api.Data;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext db;

    public ProjectRepository(AppDbContext db) => this.db = db;

    public async Task<(List<Project> Items, int Total)> Search(ProjectQuery query)
    {
        IQueryable<Project> source = db.Projects
            .AsNoTracking()
            .Include(p => p.Owner)
            .Include(p => p.Tasks);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            source = source.Where(p => p.Name.Contains(term) || p.Description.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
            source = source.Where(p => p.Status == query.Status);

        if (query.OwnerId.HasValue)
            source = source.Where(p => p.OwnerId == query.OwnerId.Value);

        source = query.SortBy.ToLower() switch
        {
            "name" => query.Desc ? source.OrderByDescending(p => p.Name) : source.OrderBy(p => p.Name),
            "status" => query.Desc ? source.OrderByDescending(p => p.Status) : source.OrderBy(p => p.Status),
            "startdate" => query.Desc ? source.OrderByDescending(p => p.StartDate) : source.OrderBy(p => p.StartDate),
            _ => query.Desc ? source.OrderByDescending(p => p.Id) : source.OrderBy(p => p.Id)
        };

        int total = await source.CountAsync();

        List<Project> items = await source
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Project?> GetById(int id) =>
        await db.Projects
            .AsNoTracking()
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Project?> GetForUpdate(int id) =>
        await db.Projects.FindAsync(id);

    public async Task<bool> AnyOwnedBy(int userId) =>
        await db.Projects.AnyAsync(p => p.OwnerId == userId);

    public void Add(Project project) => db.Projects.Add(project);

    public void Remove(Project project) => db.Projects.Remove(project);

    public async Task SaveChanges() => await db.SaveChangesAsync();
}
