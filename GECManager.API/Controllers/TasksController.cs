using GECManager.Api.Data;
using GECManager.Shared.DTOs;
using GECManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GECManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public TasksController(ApplicationDbContext db) => _db = db;

    private int CallerId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    // Admin gerencia tarefas de qualquer projeto; Leader só as dos projetos que criou
    private async Task<bool> PodeGerenciar(int projectId)
    {
        if (User.IsInRole("Admin")) return true;
        if (!User.IsInRole("Leader")) return false;
        var ownerId = await _db.Projects
            .Where(p => p.Id == projectId)
            .Select(p => p.OwnerId)
            .FirstOrDefaultAsync();
        return ownerId == CallerId;
    }

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
    [Authorize(Policy = "AdminOrLeader")]
    public async Task<ActionResult<TaskItem>> Create(TaskItemDTO dto)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == dto.ProjectId))
            return NotFound("Projeto não encontrado.");
        if (!await PodeGerenciar(dto.ProjectId)) return Forbid();

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
    [Authorize(Policy = "AdminOrLeader")]
    public async Task<IActionResult> Update(int id, TaskItemDTO dto)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        if (!await PodeGerenciar(item.ProjectId)) return Forbid();

        item.Title = dto.Title;
        item.Description = dto.Description;
        item.Status = dto.Status;
        item.AssignedUserId = dto.AssignedUserId;
        item.DueDate = dto.DueDate;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Quem gerencia o projeto ou o usuário atribuído à tarefa pode mover o status
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] TaskItemStatus status)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        if (!await PodeGerenciar(item.ProjectId) && item.AssignedUserId != CallerId)
            return Forbid();

        item.Status = status;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}/assign")]
    [Authorize(Policy = "AdminOrLeader")]
    public async Task<IActionResult> Assign(int id, [FromBody] int? userId)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        if (!await PodeGerenciar(item.ProjectId)) return Forbid();
        if (userId is int uid && !await _db.Users.AnyAsync(u => u.Id == uid))
            return NotFound("Usuário não encontrado.");

        item.AssignedUserId = userId;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOrLeader")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Tasks.FindAsync(id);
        if (item is null) return NotFound();
        if (!await PodeGerenciar(item.ProjectId)) return Forbid();

        _db.Tasks.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
