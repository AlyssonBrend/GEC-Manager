using System.Net.Http.Json;

namespace GECManager.Web.Services;

public class DashboardStats
{
    public int TotalProjects { get; set; }
    public int TotalTasks { get; set; }
    public ByStatusStats ByStatus { get; set; } = new();
    public List<MyTask> MyTasks { get; set; } = new();
}

public class ByStatusStats
{
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Done { get; set; }
}

public class MyTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public DateTime? DueDate { get; set; }
}

public class DashboardService
{
    private readonly HttpClient _http;
    public DashboardService(HttpClient http) => _http = http;

    public async Task<DashboardStats?> GetStatsAsync()
        => await _http.GetFromJsonAsync<DashboardStats>("api/dashboard");
}
