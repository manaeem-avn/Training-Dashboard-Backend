using System.Security.Claims;

namespace WorkPortal.Api.Services;

public interface ICurrentUser
{
    int Id { get; }
    string Email { get; }
    string Role { get; }
    bool IsAdmin { get; }
}

public class CurrentUser : ICurrentUser
{
    private readonly ClaimsPrincipal? user;

    public CurrentUser(IHttpContextAccessor accessor) => user = accessor.HttpContext?.User;

    public int Id => int.TryParse(user?.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 0;
    public string Email => user?.FindFirstValue(ClaimTypes.Email) ?? "anonymous";
    public string Role => user?.FindFirstValue(ClaimTypes.Role) ?? "";
    public bool IsAdmin => Role == "Admin";
}
