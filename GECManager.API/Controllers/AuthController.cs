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
        var email = dto.Email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return BadRequest("E-mail inválido.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Informe um nome.");
        if (dto.Password.Length < 6)
            return BadRequest("A senha precisa de pelo menos 6 caracteres.");
        if (await _db.Users.AnyAsync(u => u.Email == email))
            return BadRequest("Email já cadastrado.");

        // O Role do DTO é ignorado: o primeiro usuário vira Admin (bootstrap)
        // e os demais entram como Member. Promoções são feitas por um Admin
        // em PUT /api/users/{id}.
        var primeiroUsuario = !await _db.Users.AnyAsync();
        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = email,
            Role = primeiroUsuario ? "Admin" : "Member"
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
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return Unauthorized("Credenciais inválidas.");

        var secret = await _db.UserSecrets.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (secret is null || !BCrypt.Net.BCrypt.Verify(dto.Password, secret.PasswordHash))
            return Unauthorized("Credenciais inválidas.");

        var token = _tokenSvc.CreateToken(user);
        return Ok(new { token, user });
    }
}