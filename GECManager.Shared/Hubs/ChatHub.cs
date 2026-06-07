using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GECManager.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task JoinProject(string projectId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, projectId);

    public async Task LeaveProject(string projectId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);

    public async Task SendMessage(string projectId, string message)
    {
        var user = Context.User?.Identity?.Name ?? "Anônimo";
        await Clients.Group(projectId).SendAsync("ReceiveMessage", user, message, DateTime.UtcNow);
    }
}
