using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Db;
using Data.Entities;
using Shared.Scores;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Scores;

[ApiController]
[Route("api/scores")]
public partial class ScoresController : ControllerBase
{
    private readonly GameDbContext _db;

    public ScoresController(GameDbContext db)
    {
        _db = db;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Submit(SubmitScoreRequest req)
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

    [HttpGet("map/{mapKey}")]
    public async Task<ActionResult<List<ScoreDto>>> GetForMap(string mapKey)
    {
        var scores = await _db.Scores
            .Include(x => x.Player)
            .Include(x => x.Map)
            .Include(x => x.WeaponStats)
                .ThenInclude(ws => ws.Weapon)
            .Where(x => x.Map.MapKey == mapKey)
            .OrderByDescending(x => x.Value)
            .ToListAsync();

        return scores.Select(s => new ScoreDto(
            s.Id,
            s.Player.Username,
            s.Map.MapKey,
            s.Difficulty,
            s.Value,
            s.TimeMs,
            [.. s.WeaponStats.Select(ws => new WeaponStatDto(
                ws.Weapon.WeaponKey,
                ws.Damage,
                ws.Kills,
                ws.Accuracy,
                ws.CriticalAccuracy
            ))]
        )).ToList();
    }
}