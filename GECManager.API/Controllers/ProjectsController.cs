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
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public ProjectsController(ApplicationDbContext db) => _db = db;

    private int CallerId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    // Admin gerencia qualquer projeto; Leader só os que criou
    private bool PodeGerenciar(Project project) =>
        User.IsInRole("Admin") || project.OwnerId == CallerId;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        => Ok(await _db.Projects.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Project>> Get(int id)
    {
        var project = await _db.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrLeader")]
    public async Task<ActionResult<Project>> Create(ProjectDTO dto)
    {
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = userId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOrLeader")]
    public async Task<IActionResult> Update(int id, ProjectDTO dto)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is null) return NotFound();
        if (!PodeGerenciar(project)) return Forbid();
        project.Name = dto.Name;
        project.Description = dto.Description;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOrLeader")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is null) return NotFound();
        if (!PodeGerenciar(project)) return Forbid();
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
