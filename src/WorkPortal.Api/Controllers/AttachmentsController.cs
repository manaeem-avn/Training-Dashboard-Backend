using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPortal.Api.Services;

namespace WorkPortal.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService attachments;

    public AttachmentsController(IAttachmentService attachments) => this.attachments = attachments;

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        (byte[] content, string contentType, string fileName) = await attachments.Download(id);
        return File(content, string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType, fileName);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await attachments.Delete(id);
        return NoContent();
    }
}
