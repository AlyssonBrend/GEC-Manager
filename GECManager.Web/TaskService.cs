using GECManager.Shared.Models;
using System.Net.Http.Json;

public class TaskService
{
    private readonly HttpClient _http;
    public TaskService(HttpClient http) => _http = http;

    public async Task<List<ProjectTask>> GetTasksAsync() =>
        await _http.GetFromJsonAsync<List<ProjectTask>>("api/tasks");
}
