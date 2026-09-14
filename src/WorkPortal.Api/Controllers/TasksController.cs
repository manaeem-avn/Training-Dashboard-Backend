using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPortal.Api.Common;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Services;

namespace WorkPortal.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService tasks;
    private readonly IAttachmentService attachments;

    public TasksController(ITaskService tasks, IAttachmentService attachments)
    {
        this.tasks = tasks;
        this.attachments = attachments;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskDto>>> Search([FromQuery] TaskQuery query) =>
        Ok(await tasks.Search(query));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetById(int id) =>
        Ok(await tasks.GetById(id));

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(TaskRequest request)
    {
        TaskDto created = await tasks.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> Update(int id, TaskRequest request) =>
        Ok(await tasks.Update(id, request));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await tasks.Delete(id);
        return NoContent();
    }

    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<List<CommentDto>>> GetComments(int id) =>
        Ok(await tasks.GetComments(id));

    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(int id, CommentRequest request) =>
        Ok(await tasks.AddComment(id, request));

    [HttpDelete("{id:int}/comments/{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int id, int commentId)
    {
        await tasks.DeleteComment(id, commentId);
        return NoContent();
    }

    [HttpGet("{id:int}/attachments")]
    public async Task<ActionResult<List<AttachmentDto>>> GetAttachments(int id) =>
        Ok(await attachments.GetByTask(id));

    [HttpPost("{id:int}/attachments")]
    public async Task<ActionResult<AttachmentDto>> Upload(int id, IFormFile file) =>
        Ok(await attachments.Upload(id, file));
}
