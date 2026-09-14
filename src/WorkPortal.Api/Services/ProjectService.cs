using WorkPortal.Api.Common;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;
using WorkPortal.Api.Repositories;

namespace WorkPortal.Api.Services;

public interface IProjectService
{
    Task<PagedResult<ProjectDto>> Search(ProjectQuery query);
    Task<ProjectDto> GetById(int id);
    Task<ProjectDto> Create(ProjectRequest request);
    Task<ProjectDto> Update(int id, ProjectRequest request);
    Task Delete(int id);
}

public class ProjectService : IProjectService
{
    private readonly IProjectRepository projects;
    private readonly IUserRepository users;
    private readonly ICurrentUser currentUser;

    public ProjectService(IProjectRepository projects, IUserRepository users, ICurrentUser currentUser)
    {
        this.projects = projects;
        this.users = users;
        this.currentUser = currentUser;
    }

    public async Task<PagedResult<ProjectDto>> Search(ProjectQuery query)
    {
        (List<Project> items, int total) = await projects.Search(query);

        return new PagedResult<ProjectDto>
        {
            Items = items.Select(Map).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = total
        };
    }

    public async Task<ProjectDto> GetById(int id)
    {
        Project project = await projects.GetById(id)
            ?? throw new NotFoundException($"Project {id} was not found.");

        return Map(project);
    }

    public async Task<ProjectDto> Create(ProjectRequest request)
    {
        await EnsureOwnerExists(request.OwnerId);

        Project project = new()
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            Status = request.Status,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OwnerId = request.OwnerId,
            CreatedBy = currentUser.Email
        };

        projects.Add(project);
        await projects.SaveChanges();

        return await GetById(project.Id);
    }

    public async Task<ProjectDto> Update(int id, ProjectRequest request)
    {
        Project project = await projects.GetForUpdate(id)
            ?? throw new NotFoundException($"Project {id} was not found.");

        await EnsureOwnerExists(request.OwnerId);

        project.Name = request.Name.Trim();
        project.Description = request.Description;
        project.Status = request.Status;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.OwnerId = request.OwnerId;
        project.UpdatedBy = currentUser.Email;
        project.UpdatedAt = DateTime.UtcNow;

        await projects.SaveChanges();
        return await GetById(id);
    }

    public async Task Delete(int id)
    {
        Project project = await projects.GetForUpdate(id)
            ?? throw new NotFoundException($"Project {id} was not found.");

        projects.Remove(project);
        await projects.SaveChanges();
    }

    private async Task EnsureOwnerExists(int ownerId)
    {
        if (!await users.Exists(ownerId))
            throw new BadRequestException($"Owner {ownerId} does not exist.");
    }

    private static ProjectDto Map(Project p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Status = p.Status,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        OwnerId = p.OwnerId,
        OwnerName = p.Owner?.FullName ?? string.Empty,
        TaskCount = p.Tasks.Count,
        CreatedBy = p.CreatedBy,
        CreatedAt = p.CreatedAt,
        UpdatedBy = p.UpdatedBy,
        UpdatedAt = p.UpdatedAt
    };
}
