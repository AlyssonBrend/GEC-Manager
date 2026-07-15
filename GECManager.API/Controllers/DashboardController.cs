using GECManager.Api.Data;
using GECManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GECManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public DashboardController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        var totalProjects = await _db.Projects.CountAsync();
        var totalTasks = await _db.Tasks.CountAsync();
        var pending = await _db.Tasks.CountAsync(t => t.Status == TaskItemStatus.Pending);
        var inProgress = await _db.Tasks.CountAsync(t => t.Status == TaskItemStatus.InProgress);
        var done = await _db.Tasks.CountAsync(t => t.Status == TaskItemStatus.Done);
        var myTasks = await _db.Tasks
            .Where(t => t.AssignedUserId == userId)
            .Select(t => new { t.Id, t.Title, t.Status, t.ProjectId, t.DueDate })
            .ToListAsync();

        return Ok(new
        {
            totalProjects,
            totalTasks,
            byStatus = new { pending, inProgress, done },
            myTasks
        });
    }
}
