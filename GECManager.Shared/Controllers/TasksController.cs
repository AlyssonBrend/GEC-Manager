using GECManager.Api.Data;
using GECManager.Shared.DTOs;
using GECManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GECManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public TasksController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
        => Ok(await _db.Tasks.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> Get(int id)
    {
        var item = await _db.Tasks.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("byProject/{projectId:int}")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetByProject(int projectId)
        => Ok(await _db.Tasks.Where(t => t.ProjectId == projectId).AsNoTracking().ToListAsync());

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(TaskItemDTO dto)
    {
        var item = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            ProjectId = dto.ProjectId,
            AssignedUserId = dto.AssignedUserId,
            DueDate = dto.DueDate
        };
        _db.Tasks.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskItemDTO dto)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        item.Title = dto.Title;
        item.Description = dto.Description;
        item.Status = dto.Status;
        item.AssignedUserId = dto.AssignedUserId;
        item.DueDate = dto.DueDate;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] TaskItemStatus status)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        item.Status = status;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> Assign(int id, [FromBody] int? userId)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        item.AssignedUserId = userId;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        _db.Tasks.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
