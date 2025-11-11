using GECManager.Api.Data;
using GECManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GECManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public UsersController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
        => Ok(await _db.Users.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await _db.Users.FindAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, User updated)
    {
        if (id != updated.Id) return BadRequest();
        _db.Entry(updated).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}