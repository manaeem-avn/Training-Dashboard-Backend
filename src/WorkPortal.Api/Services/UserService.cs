using Microsoft.AspNetCore.Identity;
using WorkPortal.Api.Common;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;
using WorkPortal.Api.Repositories;

namespace WorkPortal.Api.Services;

public interface IUserService
{
    Task<PagedResult<UserDto>> Search(PagedQuery query);
    Task<List<UserDto>> GetAll();
    Task<UserDto> GetById(int id);
    Task<UserDto> Create(UserRequest request);
    Task<UserDto> Update(int id, UserRequest request);
    Task Delete(int id);
}

public class UserService : IUserService
{
    private readonly IUserRepository users;
    private readonly IProjectRepository projects;
    private readonly PasswordHasher<AppUser> hasher = new();

    public UserService(IUserRepository users, IProjectRepository projects)
    {
        this.users = users;
        this.projects = projects;
    }

    public async Task<PagedResult<UserDto>> Search(PagedQuery query)
    {
        (List<AppUser> items, int total) = await users.Search(query);

        return new PagedResult<UserDto>
        {
            Items = items.Select(Map).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = total
        };
    }

    public async Task<List<UserDto>> GetAll() =>
        (await users.GetAll()).Select(Map).ToList();

    public async Task<UserDto> GetById(int id)
    {
        AppUser user = await users.GetById(id)
            ?? throw new NotFoundException($"User {id} was not found.");

        return Map(user);
    }

    public async Task<UserDto> Create(UserRequest request)
    {
        string email = request.Email.Trim().ToLower();

        if (await users.EmailTaken(email))
            throw new BadRequestException("Email is already registered.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Password is required for a new user.");

        AppUser user = new()
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Role = request.Role,
            IsActive = request.IsActive
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        users.Add(user);
        await users.SaveChanges();

        return Map(user);
    }

    public async Task<UserDto> Update(int id, UserRequest request)
    {
        AppUser user = await users.GetForUpdate(id)
            ?? throw new NotFoundException($"User {id} was not found.");

        string email = request.Email.Trim().ToLower();

        if (await users.EmailTaken(email, id))
            throw new BadRequestException("Email is already registered.");

        user.FullName = request.FullName.Trim();
        user.Email = email;
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = hasher.HashPassword(user, request.Password);

        await users.SaveChanges();
        return Map(user);
    }

    public async Task Delete(int id)
    {
        AppUser user = await users.GetForUpdate(id)
            ?? throw new NotFoundException($"User {id} was not found.");

        if (await projects.AnyOwnedBy(id))
            throw new BadRequestException("This user owns projects and cannot be deleted.");

        users.Remove(user);
        await users.SaveChanges();
    }

    private static UserDto Map(AppUser u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Role = u.Role,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
