using System.Net.Http.Json;
using GECManager.Shared.DTOs;
using GECManager.Shared.Models;

namespace GECManager.Web.Services;

public class ProjectService
{
    private readonly HttpClient _http;
    public ProjectService(HttpClient http) => _http = http;

    public async Task<List<Project>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<Project>>("api/projects") ?? new();

    public async Task<Project?> GetAsync(int id)
        => await _http.GetFromJsonAsync<Project>($"api/projects/{id}");

    public async Task<bool> CreateAsync(ProjectDTO dto)
    {
        var res = await _http.PostAsJsonAsync("api/projects", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(int id, ProjectDTO dto)
    {
        var res = await _http.PutAsJsonAsync($"api/projects/{id}", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var res = await _http.DeleteAsync($"api/projects/{id}");
        return res.IsSuccessStatusCode;
    }
}
