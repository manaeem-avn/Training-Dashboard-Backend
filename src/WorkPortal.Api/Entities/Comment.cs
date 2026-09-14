namespace WorkPortal.Api.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int TaskId { get; set; }
    public WorkTask Task { get; set; } = null!;
    public int AuthorId { get; set; }
    public AppUser Author { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
