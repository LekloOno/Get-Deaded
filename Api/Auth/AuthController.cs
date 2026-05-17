using Microsoft.AspNetCore.Mvc;
using Data.Db;
using Microsoft.EntityFrameworkCore;
using Shared.Auth;
using Data.Entities;

namespace Api.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly GameDbContext _db;

    public AuthController(GameDbContext db)
    {
        _db = db;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var normalizedUsername = req.Username.Trim().ToLowerInvariant();

        var player = await _db.Players
            .FirstOrDefaultAsync(x => x.Username == normalizedUsername);

        if (player == null)
            return Unauthorized();

        var valid = BCrypt.Net.BCrypt.Verify(req.Password, player.PasswordHash);

        if (!valid)
            return Unauthorized();

        var token = "TODO_JWT";

        return new AuthResponse(token, player.Username);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username))
            return BadRequest("Username is required.");

        if (string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Password is required.");

        var normalizedUsername = req.Username.Trim().ToLowerInvariant();

        var usernameExists = await _db.Players
            .AnyAsync(x => x.Username == normalizedUsername);

        if (usernameExists)
            return Conflict("Username already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        var player = new Player
        {
            Id = Guid.NewGuid(),
            Username = normalizedUsername,
            PasswordHash = passwordHash
        };

        _db.Players.Add(player);

        await _db.SaveChangesAsync();

        var token = "TODO_JWT";

        return Ok(new AuthResponse(
            token,
            player.Username
        ));
    }
}