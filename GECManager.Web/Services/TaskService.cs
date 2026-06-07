using System.Net.Http.Json;
using GECManager.Shared.DTOs;
using GECManager.Shared.Models;

namespace GECManager.Web.Services;

public class TaskService
{
    private readonly HttpClient _http;
    public TaskService(HttpClient http) => _http = http;

    public async Task<List<TaskItem>> GetByProjectAsync(int projectId)
        => await _http.GetFromJsonAsync<List<TaskItem>>($"api/tasks/byProject/{projectId}") ?? new();

    public async Task<bool> CreateAsync(TaskItemDTO dto)
    {
        var res = await _http.PostAsJsonAsync("api/tasks", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStatusAsync(int taskId, TaskItemStatus status)
    {
        var res = await _http.PatchAsJsonAsync($"api/tasks/{taskId}/status", status);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> AssignAsync(int taskId, int? userId)
    {
        var res = await _http.PatchAsJsonAsync($"api/tasks/{taskId}/assign", userId);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var res = await _http.DeleteAsync($"api/tasks/{id}");
        return res.IsSuccessStatusCode;
    }
}
