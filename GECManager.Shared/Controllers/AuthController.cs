using BCrypt.Net;
using GECManager.Api.Data;
using GECManager.Api.Entities;
using GECManager.Api.Services;
using GECManager.Shared.DTOs;
using GECManager.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GECManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ITokenService _tokenSvc;

    public AuthController(ApplicationDbContext db, ITokenService tokenSvc)
    {
        _db = db;
        _tokenSvc = tokenSvc;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserRegisterDTO dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email já cadastrado.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = "User"
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await _db.UserSecrets.AddAsync(new UserSecret { UserId = user.Id, PasswordHash = hash });
        await _db.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login(UserLoginDTO dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null) return Unauthorized("Credenciais inválidas.");

        var secret = await _db.UserSecrets.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (secret is null || !BCrypt.Net.BCrypt.Verify(dto.Password, secret.PasswordHash))
            return Unauthorized("Credenciais inválidas.");

        var token = _tokenSvc.CreateToken(user);
        return Ok(new { token, user });
    }
}