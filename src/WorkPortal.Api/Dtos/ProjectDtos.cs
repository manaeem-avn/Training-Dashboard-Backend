using System.ComponentModel.DataAnnotations;

namespace WorkPortal.Api.Dtos;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ProjectRequest : IValidatableObject
{
    [Required, StringLength(120, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required, RegularExpression("Active|OnHold|Completed", ErrorMessage = "Status must be Active, OnHold or Completed.")]
    public string Status { get; set; } = "Active";

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "OwnerId is required.")]
    public int OwnerId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (EndDate.HasValue && EndDate.Value < StartDate)
            yield return new ValidationResult("EndDate cannot be before StartDate.", new[] { nameof(EndDate) });
    }
}

public class ProjectQuery : PagedQuery
{
    public string? Status { get; set; }
    public int? OwnerId { get; set; }
}
