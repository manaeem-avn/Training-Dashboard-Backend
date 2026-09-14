using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Repositories;

public interface IUserRepository
{
    Task<(List<AppUser> Items, int Total)> Search(PagedQuery query);
    Task<List<AppUser>> GetAll();
    Task<AppUser?> GetById(int id);
    Task<AppUser?> GetForUpdate(int id);
    Task<AppUser?> GetByEmail(string email);
    Task<bool> Exists(int id);
    Task<bool> EmailTaken(string email, int? excludingId = null);
    void Add(AppUser user);
    void Remove(AppUser user);
    Task SaveChanges();
}
