using Data.Db;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Auth;

namespace Api.Auth;

public class AuthService : IAuthService
{
    private readonly GameDbContext _db;
    private readonly IJwtTokenService _jwt;

    public AuthService(GameDbContext db, IJwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<AuthOutcome> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var username = Normalize(request.Username);

        if (await _db.Players.AnyAsync(p => p.Username == username, ct))
            return new AuthOutcome(AuthOutcomeStatus.UsernameTaken);

        var player = new Player
        {
            Id = Guid.NewGuid(),
            Username = username,
            DisplayName = request.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _db.Players.Add(player);
        await _db.SaveChangesAsync(ct);

        var token = _jwt.CreateToken(player);
        return new AuthOutcome(AuthOutcomeStatus.Success, new AuthResponse(token, player.Id, player.Username, player.DisplayName));
    }

    public async Task<AuthOutcome> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var username = Normalize(request.Username);
        var player = await _db.Players.FirstOrDefaultAsync(p => p.Username == username, ct);

        if (player is null || !BCrypt.Net.BCrypt.Verify(request.Password, player.PasswordHash))
            return new AuthOutcome(AuthOutcomeStatus.InvalidCredentials);

        var token = _jwt.CreateToken(player);
        return new AuthOutcome(AuthOutcomeStatus.Success, new AuthResponse(token, player.Id, player.Username, player.DisplayName));
    }

    private static string Normalize(string username) => username.Trim().ToLowerInvariant();
}