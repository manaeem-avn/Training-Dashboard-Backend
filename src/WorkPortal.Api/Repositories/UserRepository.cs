using Microsoft.EntityFrameworkCore;
using WorkPortal.Api.Data;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext db;

    public UserRepository(AppDbContext db) => this.db = db;

    public async Task<(List<AppUser> Items, int Total)> Search(PagedQuery query)
    {
        IQueryable<AppUser> source = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            source = source.Where(u => u.FullName.Contains(term) || u.Email.Contains(term));
        }

        source = query.SortBy.ToLower() switch
        {
            "fullname" => query.Desc ? source.OrderByDescending(u => u.FullName) : source.OrderBy(u => u.FullName),
            "email" => query.Desc ? source.OrderByDescending(u => u.Email) : source.OrderBy(u => u.Email),
            "role" => query.Desc ? source.OrderByDescending(u => u.Role) : source.OrderBy(u => u.Role),
            _ => query.Desc ? source.OrderByDescending(u => u.Id) : source.OrderBy(u => u.Id)
        };

        int total = await source.CountAsync();

        List<AppUser> items = await source
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<AppUser>> GetAll() =>
        await db.Users.AsNoTracking().OrderBy(u => u.FullName).ToListAsync();

    public async Task<AppUser?> GetById(int id) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<AppUser?> GetForUpdate(int id) =>
        await db.Users.FindAsync(id);

    public async Task<AppUser?> GetByEmail(string email) =>
        await db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> Exists(int id) =>
        await db.Users.AnyAsync(u => u.Id == id);

    public async Task<bool> EmailTaken(string email, int? excludingId = null) =>
        await db.Users.AnyAsync(u => u.Email == email && (excludingId == null || u.Id != excludingId));

    public void Add(AppUser user) => db.Users.Add(user);

    public void Remove(AppUser user) => db.Users.Remove(user);

    public async Task SaveChanges() => await db.SaveChangesAsync();
}
