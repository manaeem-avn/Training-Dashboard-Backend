using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPortal.Api.Dtos;
using WorkPortal.Api.Services;

namespace WorkPortal.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService dashboard;

    public DashboardController(IDashboardService dashboard) => this.dashboard = dashboard;

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardDto>> Summary() =>
        Ok(await dashboard.GetSummary());
}
