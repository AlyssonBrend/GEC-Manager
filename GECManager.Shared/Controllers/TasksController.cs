using GECManager.Api.Data;
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

    [HttpGet("byProject/{projectId:int}")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetByProject(int projectId)
        => Ok(await _db.Tasks.Where(t => t.ProjectId == projectId).AsNoTracking().ToListAsync());

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(TaskItem item)
    {
        _db.Tasks.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskItem item)
    {
        if (id != item.Id) return BadRequest();
        _db.Entry(item).State = EntityState.Modified;
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