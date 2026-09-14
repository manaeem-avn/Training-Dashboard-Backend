using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPortal.Api.Common;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Services;

namespace WorkPortal.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService projects;

    public ProjectsController(IProjectService projects) => this.projects = projects;

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProjectDto>>> Search([FromQuery] ProjectQuery query) =>
        Ok(await projects.Search(query));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id) =>
        Ok(await projects.GetById(id));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProjectDto>> Create(ProjectRequest request)
    {
        ProjectDto created = await projects.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProjectDto>> Update(int id, ProjectRequest request) =>
        Ok(await projects.Update(id, request));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await projects.Delete(id);
        return NoContent();
    }
}
