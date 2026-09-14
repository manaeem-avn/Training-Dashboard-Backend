using System.ComponentModel.DataAnnotations;

namespace WorkPortal.Api.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserRequest
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, RegularExpression("Admin|User", ErrorMessage = "Role must be Admin or User.")]
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;

    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }
}
