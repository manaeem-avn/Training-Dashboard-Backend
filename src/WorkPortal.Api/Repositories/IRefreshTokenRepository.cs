using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByToken(string token);
    void Add(RefreshToken token);
    Task SaveChanges();
}
