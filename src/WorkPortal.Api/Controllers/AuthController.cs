using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Services;

namespace WorkPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService auth;
    private readonly ICurrentUser currentUser;

    public AuthController(IAuthService auth, ICurrentUser currentUser)
    {
        this.auth = auth;
        this.currentUser = currentUser;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request) =>
        Ok(await auth.Register(request));

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request) =>
        Ok(await auth.Login(request));

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request) =>
        Ok(await auth.Refresh(request.RefreshToken));

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(RefreshRequest request)
    {
        await auth.Logout(request.RefreshToken);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> Me() =>
        Ok(new { currentUser.Id, currentUser.Email, currentUser.Role });
}
