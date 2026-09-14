namespace WorkPortal.Api.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public List<Project> OwnedProjects { get; set; } = new();
    public List<WorkTask> AssignedTasks { get; set; } = new();
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
