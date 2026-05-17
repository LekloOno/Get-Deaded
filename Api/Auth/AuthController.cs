using Microsoft.AspNetCore.Mvc;
using Data.Db;
using Microsoft.EntityFrameworkCore;
using Shared.Auth;

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
        var player = await _db.Players
            .FirstOrDefaultAsync(x => x.Username == req.Username);

        if (player == null)
            return Unauthorized();

        var valid = BCrypt.Net.BCrypt.Verify(req.Password, player.PasswordHash);

        if (!valid)
            return Unauthorized();

        var token = "TODO_JWT";

        return new AuthResponse(token, player.Username);
    }
}