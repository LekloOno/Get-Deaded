using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Db;
using Data.Entities;
using Shared.Scores;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Api.Context;

namespace Api.Scores;

[ApiController]
[Route("api/scores")]
public partial class ScoresController : ControllerBase
{
    private readonly GameDbContext _db;
    private readonly GameSession _session;

    public ScoresController(GameDbContext db, GameSession session)
    {
        _db = db;
        _session = session;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Submit<T>(SubmitScoreRequest<T> req)
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(sub, out var playerId))
            return Unauthorized();

        var player = await _db.Players
            .FirstAsync(x => x.Id == playerId);

        var mapExists = await _db.Maps
            .AnyAsync(x => x.MapKey == req.MapKey);

        if (!mapExists)
            return BadRequest("Invalid map key");

        var score = new Score
        {
            PlayerId = player.Id,
            MapKey = req.MapKey,
            ClientVersionKey = _session.Version.VersionKey,
            Difficulty = req.Difficulty,
            TimeMs = req.TimeMs,
            Value = req.Score,

            WeaponStats = [.. req.WeaponStats.Select(ws => new WeaponStat
            {
                WeaponKey = ws.WeaponKey,
                Damage = ws.Damage,
                Kills = ws.Kills,
                Accuracy = ws.Accuracy,
                CriticalAccuracy = ws.CriticalAccuracy
            })]
        };

        _db.Scores.Add(score);
        await _db.SaveChangesAsync();

        var rank = await _db.Scores
            .Where(x =>
                x.MapKey == req.MapKey &&
                x.Difficulty == req.Difficulty &&
                x.Value > req.Score)
            .CountAsync() + 1;

        return Ok(new SubmitScoreResponse(
            score.Id,
            rank
        ));
    }
}