namespace WorkPortal.Api.Dtos;

public class DashboardDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int TotalTasks { get; set; }
    public int OpenTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TotalUsers { get; set; }
    public List<ChartPoint> TasksByStatus { get; set; } = new();
    public List<ChartPoint> TasksByPriority { get; set; } = new();
    public List<ChartPoint> TasksPerProject { get; set; } = new();
}

public class ChartPoint
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
}
