namespace WorkPortal.Api.Repositories;

public record CountByLabel(string Label, int Value);

public interface IDashboardRepository
{
    Task<int> CountProjects(string? status = null);
    Task<int> CountTasks();
    Task<int> CountOpenTasks();
    Task<int> CountOverdueTasks(DateTime today);
    Task<int> CountUsers();
    Task<List<CountByLabel>> TasksByStatus();
    Task<List<CountByLabel>> TasksByPriority();
    Task<List<CountByLabel>> TasksPerProject();
}
