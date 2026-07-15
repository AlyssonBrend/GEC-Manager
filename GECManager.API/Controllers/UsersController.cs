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
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public UsersController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Policy = "AdminOrLeader")]
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

        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        var callerId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var cid) ? cid : 0;
        var isAdmin = User.IsInRole("Admin");
        if (callerId != id && !isAdmin) return Forbid();

        var email = updated.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email && u.Id != id))
            return Conflict("Este e-mail já está em uso.");

        user.Name = updated.Name.Trim();
        user.Email = email;

        // Papel só muda pela mão de um Admin — bloqueia auto-promoção
        if (updated.Role != user.Role)
        {
            if (!isAdmin) return Forbid();
            if (updated.Role is not ("Admin" or "Leader" or "Member"))
                return BadRequest("Papel inválido. Use Admin, Leader ou Member.");
            user.Role = updated.Role;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
