using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using GECManager.Shared.DTOs;
using GECManager.Shared.Models;
using Microsoft.JSInterop;

namespace GECManager.Web.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync("api/auth/login", new UserLoginDTO { Email = email, Password = password });
        if (!res.IsSuccessStatusCode) return false;
        var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (data?.Token is null) return false;
        await _js.InvokeVoidAsync("localStorage.setItem", "jwt_token", data.Token);
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", data.Token);
        return true;
    }

    public async Task<bool> RegisterAsync(UserRegisterDTO dto)
    {
        var res = await _http.PostAsJsonAsync("api/auth/register", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task LogoutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "jwt_token");
        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<bool> TryRestoreTokenAsync()
    {
        var token = await _js.InvokeAsync<string?>("localStorage.getItem", "jwt_token");
        if (string.IsNullOrEmpty(token)) return false;
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return true;
    }

    public async Task<(int Id, string Name, string Role)?> GetCurrentUserAsync()
    {
        var token = await _js.InvokeAsync<string?>("localStorage.getItem", "jwt_token");
        if (string.IsNullOrEmpty(token)) return null;
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var id = int.TryParse(jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, out var i) ? i : 0;
        var name = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
        var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Member";
        return (id, name, role);
    }

    private record LoginResponse(string Token, User User);
}
