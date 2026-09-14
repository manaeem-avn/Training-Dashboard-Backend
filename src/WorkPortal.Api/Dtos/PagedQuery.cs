using System.ComponentModel.DataAnnotations;

namespace WorkPortal.Api.Dtos;

public class PagedQuery
{
    public string? Search { get; set; }
    public string SortBy { get; set; } = "id";
    public bool Desc { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 10;
}
