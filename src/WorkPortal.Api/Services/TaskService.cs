using WorkPortal.Api.Common;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;
using WorkPortal.Api.Repositories;

namespace WorkPortal.Api.Services;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> Search(TaskQuery query);
    Task<TaskDto> GetById(int id);
    Task<TaskDto> Create(TaskRequest request);
    Task<TaskDto> Update(int id, TaskRequest request);
    Task Delete(int id);
    Task<List<CommentDto>> GetComments(int taskId);
    Task<CommentDto> AddComment(int taskId, CommentRequest request);
    Task DeleteComment(int taskId, int commentId);
}

public class TaskService : ITaskService
{
    private readonly ITaskRepository tasks;
    private readonly IProjectRepository projects;
    private readonly IUserRepository users;
    private readonly ICurrentUser currentUser;

    public TaskService(
        ITaskRepository tasks,
        IProjectRepository projects,
        IUserRepository users,
        ICurrentUser currentUser)
    {
        this.tasks = tasks;
        this.projects = projects;
        this.users = users;
        this.currentUser = currentUser;
    }

    public async Task<PagedResult<TaskDto>> Search(TaskQuery query)
    {
        (List<WorkTask> items, int total) = await tasks.Search(query);

        return new PagedResult<TaskDto>
        {
            Items = items.Select(Map).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = total
        };
    }

    public async Task<TaskDto> GetById(int id)
    {
        WorkTask task = await tasks.GetById(id)
            ?? throw new NotFoundException($"Task {id} was not found.");

        return Map(task);
    }

    public async Task<TaskDto> Create(TaskRequest request)
    {
        await EnsureReferencesExist(request);

        WorkTask task = new()
        {
            Title = request.Title.Trim(),
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId,
            AssigneeId = request.AssigneeId,
            CreatedBy = currentUser.Email
        };

        tasks.Add(task);
        await tasks.SaveChanges();

        return await GetById(task.Id);
    }

    public async Task<TaskDto> Update(int id, TaskRequest request)
    {
        WorkTask task = await tasks.GetForUpdate(id)
            ?? throw new NotFoundException($"Task {id} was not found.");

        await EnsureReferencesExist(request);

        task.Title = request.Title.Trim();
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.ProjectId = request.ProjectId;
        task.AssigneeId = request.AssigneeId;
        task.UpdatedBy = currentUser.Email;
        task.UpdatedAt = DateTime.UtcNow;

        await tasks.SaveChanges();
        return await GetById(id);
    }

    public async Task Delete(int id)
    {
        WorkTask task = await tasks.GetForUpdate(id)
            ?? throw new NotFoundException($"Task {id} was not found.");

        tasks.Remove(task);
        await tasks.SaveChanges();
    }

    public async Task<List<CommentDto>> GetComments(int taskId)
    {
        await EnsureTaskExists(taskId);

        return (await tasks.GetComments(taskId)).Select(MapComment).ToList();
    }

    public async Task<CommentDto> AddComment(int taskId, CommentRequest request)
    {
        await EnsureTaskExists(taskId);

        Comment comment = new()
        {
            Text = request.Text.Trim(),
            TaskId = taskId,
            AuthorId = currentUser.Id
        };

        tasks.AddComment(comment);
        await tasks.SaveChanges();

        return (await GetComments(taskId)).First(c => c.Id == comment.Id);
    }

    public async Task DeleteComment(int taskId, int commentId)
    {
        Comment comment = await tasks.GetComment(taskId, commentId)
            ?? throw new NotFoundException($"Comment {commentId} was not found.");

        if (!currentUser.IsAdmin && comment.AuthorId != currentUser.Id)
            throw new BadRequestException("You can only delete your own comments.");

        tasks.RemoveComment(comment);
        await tasks.SaveChanges();
    }

    private async Task EnsureTaskExists(int taskId)
    {
        if (!await tasks.Exists(taskId))
            throw new NotFoundException($"Task {taskId} was not found.");
    }

    private async Task EnsureReferencesExist(TaskRequest request)
    {
        if (await projects.GetForUpdate(request.ProjectId) is null)
            throw new BadRequestException($"Project {request.ProjectId} does not exist.");

        if (request.AssigneeId.HasValue && !await users.Exists(request.AssigneeId.Value))
            throw new BadRequestException($"User {request.AssigneeId} does not exist.");
    }

    private static TaskDto Map(WorkTask t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status,
        Priority = t.Priority,
        DueDate = t.DueDate,
        ProjectId = t.ProjectId,
        ProjectName = t.Project?.Name ?? string.Empty,
        AssigneeId = t.AssigneeId,
        AssigneeName = t.Assignee?.FullName,
        CommentCount = t.Comments.Count,
        AttachmentCount = t.Attachments.Count,
        CreatedBy = t.CreatedBy,
        CreatedAt = t.CreatedAt,
        UpdatedBy = t.UpdatedBy,
        UpdatedAt = t.UpdatedAt
    };

    private static CommentDto MapComment(Comment c) => new()
    {
        Id = c.Id,
        Text = c.Text,
        TaskId = c.TaskId,
        AuthorId = c.AuthorId,
        AuthorName = c.Author?.FullName ?? string.Empty,
        CreatedAt = c.CreatedAt
    };
}
