using GECManager.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GECManager.Api.Hubs;

[Authorize]
public class ChatHub(ApplicationDbContext db) : Hub
{
    public async Task JoinProject(string projectId)
    {
        if (!int.TryParse(projectId, out var pid)) return;
        var exists = await db.Projects.AnyAsync(p => p.Id == pid);
        if (!exists) return;
        await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
    }

    public async Task LeaveProject(string projectId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);

    public async Task SendMessage(string projectId, string message)
    {
        var user = Context.User?.Identity?.Name ?? "Anônimo";
        await Clients.Group(projectId).SendAsync("ReceiveMessage", user, message, DateTime.UtcNow);
    }
}
