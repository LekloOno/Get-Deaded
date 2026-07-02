using Microsoft.AspNetCore.Mvc;
using Shared.Scores;

namespace Api.Scores.Test;

public partial class ScoresController : ControllerBase
{
    [HttpGet("map/{mapKey}")]
    public async Task<ActionResult<List<ScoreDto<TestDetails>>>> GetForMap(string mapKey, string modeKey)
    {
        var scores = await _db.Scores
            .Include(x => x.Player)
            .Include(x => x.Map)
            .Include(x => x.WeaponStats)
                .ThenInclude(ws => ws.Weapon)
            .Where(x => x.Map.MapKey == mapKey)
            .Where(x => x.Mode.ModeKey == modeKey)
            .OrderByDescending(x => x.Value)
            .ToListAsync();

        return scores.Select(s => new ScoreDto<TestDetails>(
            s.Id,
            s.Player.Username,
            s.Map.MapKey,
            s.Mode.ModeKey,
            _session.Version.VersionKey,
            s.Difficulty,
            s.Value,
            s.TimeMs,
            [.. s.WeaponStats.Select(ws => new WeaponStatDto(
                ws.Weapon.WeaponKey,
                ws.Damage,
                ws.Kills,
                ws.Accuracy,
                ws.CriticalAccuracy
            ))],
            new()
        )).ToList();
    }
}