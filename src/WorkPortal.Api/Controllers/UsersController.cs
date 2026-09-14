using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPortal.Api.Common;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Services;

namespace WorkPortal.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService users;

    public UsersController(IUserService users) => this.users = users;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedResult<UserDto>>> Search([FromQuery] PagedQuery query) =>
        Ok(await users.Search(query));

    [HttpGet("lookup")]
    public async Task<ActionResult<List<UserDto>>> Lookup() =>
        Ok(await users.GetAll());

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetById(int id) =>
        Ok(await users.GetById(id));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> Create(UserRequest request)
    {
        UserDto created = await users.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> Update(int id, UserRequest request) =>
        Ok(await users.Update(id, request));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await users.Delete(id);
        return NoContent();
    }
}
