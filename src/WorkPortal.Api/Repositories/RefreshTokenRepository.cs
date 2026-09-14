using Microsoft.EntityFrameworkCore;
using WorkPortal.Api.Data;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext db;

    public RefreshTokenRepository(AppDbContext db) => this.db = db;

    public async Task<RefreshToken?> GetByToken(string token) =>
        await db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

    public void Add(RefreshToken token) => db.RefreshTokens.Add(token);

    public async Task SaveChanges() => await db.SaveChangesAsync();
}
