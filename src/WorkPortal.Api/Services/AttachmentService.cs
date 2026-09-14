using WorkPortal.Api.Common;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Entities;
using WorkPortal.Api.Repositories;

namespace WorkPortal.Api.Services;

public interface IAttachmentService
{
    Task<List<AttachmentDto>> GetByTask(int taskId);
    Task<AttachmentDto> Upload(int taskId, IFormFile file);
    Task<(byte[] Content, string ContentType, string FileName)> Download(int id);
    Task Delete(int id);
}

public class AttachmentService : IAttachmentService
{
    private static readonly string[] AllowedExtensions =
        { ".pdf", ".png", ".jpg", ".jpeg", ".txt", ".csv", ".docx", ".xlsx" };

    private const long MaxBytes = 5 * 1024 * 1024;

    private readonly IAttachmentRepository attachments;
    private readonly ITaskRepository tasks;
    private readonly ICurrentUser currentUser;
    private readonly string folder;

    public AttachmentService(
        IAttachmentRepository attachments,
        ITaskRepository tasks,
        ICurrentUser currentUser,
        IWebHostEnvironment env)
    {
        this.attachments = attachments;
        this.tasks = tasks;
        this.currentUser = currentUser;
        folder = Path.Combine(env.ContentRootPath, "Uploads");
        Directory.CreateDirectory(folder);
    }

    public async Task<List<AttachmentDto>> GetByTask(int taskId) =>
        (await attachments.GetByTask(taskId)).Select(Map).ToList();

    public async Task<AttachmentDto> Upload(int taskId, IFormFile file)
    {
        if (!await tasks.Exists(taskId))
            throw new NotFoundException($"Task {taskId} was not found.");

        if (file is null || file.Length == 0)
            throw new BadRequestException("File is empty.");

        if (file.Length > MaxBytes)
            throw new BadRequestException("File exceeds the 5 MB limit.");

        string extension = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(extension))
            throw new BadRequestException($"File type {extension} is not allowed.");

        string storedName = $"{Guid.NewGuid()}{extension}";
        string path = Path.Combine(folder, storedName);

        await using (FileStream stream = File.Create(path))
            await file.CopyToAsync(stream);

        Attachment attachment = new()
        {
            FileName = Path.GetFileName(file.FileName),
            StoredName = storedName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            TaskId = taskId,
            UploadedBy = currentUser.Email
        };

        attachments.Add(attachment);
        await attachments.SaveChanges();

        return Map(attachment);
    }

    public async Task<(byte[] Content, string ContentType, string FileName)> Download(int id)
    {
        Attachment attachment = await attachments.GetById(id)
            ?? throw new NotFoundException($"Attachment {id} was not found.");

        string path = Path.Combine(folder, Path.GetFileName(attachment.StoredName));
        if (!File.Exists(path))
            throw new NotFoundException("The stored file is missing.");

        byte[] content = await File.ReadAllBytesAsync(path);
        return (content, attachment.ContentType, attachment.FileName);
    }

    public async Task Delete(int id)
    {
        Attachment attachment = await attachments.GetById(id)
            ?? throw new NotFoundException($"Attachment {id} was not found.");

        string path = Path.Combine(folder, Path.GetFileName(attachment.StoredName));
        if (File.Exists(path)) File.Delete(path);

        attachments.Remove(attachment);
        await attachments.SaveChanges();
    }

    private static AttachmentDto Map(Attachment a) => new()
    {
        Id = a.Id,
        FileName = a.FileName,
        ContentType = a.ContentType,
        SizeBytes = a.SizeBytes,
        TaskId = a.TaskId,
        UploadedBy = a.UploadedBy,
        CreatedAt = a.CreatedAt
    };
}
