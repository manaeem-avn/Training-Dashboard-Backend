using System.ComponentModel.DataAnnotations;

namespace WorkPortal.Api.Dtos;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TaskRequest
{
    [Required, StringLength(150, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required, RegularExpression("Todo|InProgress|Done", ErrorMessage = "Status must be Todo, InProgress or Done.")]
    public string Status { get; set; } = "Todo";

    [Required, RegularExpression("Low|Medium|High", ErrorMessage = "Priority must be Low, Medium or High.")]
    public string Priority { get; set; } = "Medium";

    public DateTime? DueDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ProjectId is required.")]
    public int ProjectId { get; set; }

    public int? AssigneeId { get; set; }
}

public class TaskQuery : PagedQuery
{
    public int? ProjectId { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public int? AssigneeId { get; set; }
}

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int TaskId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CommentRequest
{
    [Required, StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

public class AttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int TaskId { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
