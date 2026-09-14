using WorkPortal.Api.Dtos;
using WorkPortal.Api.Repositories;

namespace WorkPortal.Api.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetSummary();
}

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository dashboard;

    public DashboardService(IDashboardRepository dashboard) => this.dashboard = dashboard;

    public async Task<DashboardDto> GetSummary()
    {
        DateTime today = DateTime.UtcNow.Date;

        return new DashboardDto
        {
            TotalProjects = await dashboard.CountProjects(),
            ActiveProjects = await dashboard.CountProjects("Active"),
            TotalTasks = await dashboard.CountTasks(),
            OpenTasks = await dashboard.CountOpenTasks(),
            OverdueTasks = await dashboard.CountOverdueTasks(today),
            TotalUsers = await dashboard.CountUsers(),
            TasksByStatus = Map(await dashboard.TasksByStatus()),
            TasksByPriority = Map(await dashboard.TasksByPriority()),
            TasksPerProject = Map(await dashboard.TasksPerProject())
        };
    }

    private static List<ChartPoint> Map(List<CountByLabel> rows) =>
        rows.Select(r => new ChartPoint { Label = r.Label, Value = r.Value }).ToList();
}
