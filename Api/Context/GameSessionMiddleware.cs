using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Api.Context;

public sealed class GameSessionMiddleware
{
    private readonly RequestDelegate _next;

    public GameSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context,
        GameDbContext db,
        GameSession session)
    {
        if (!context.Request.Headers.TryGetValue(
                "X-Client-Version",
                out var versionKey))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(
                "Missing X-Client-Version header.");
            return;
        }

        var version = await db.GameVersions
            .Include(v => v.CompatibilityGroup)
            .SingleOrDefaultAsync(
                v => v.VersionKey == versionKey);

        if (version == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(
                $"Unsupported client version '{versionKey}'.");
            return;
        }

        session.Version = version;

        var sub = context.User.FindFirstValue(
            JwtRegisteredClaimNames.Sub);

        if (Guid.TryParse(sub, out var playerId))
        {
            session.Player = await db.Players
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == playerId);
        }

        await _next(context);
    }
}