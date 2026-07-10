using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Scores;
namespace Api.Scores;

public partial class ScoresController : ControllerBase
{
    [HttpGet("leaderboard/unique")]
    public async Task<ActionResult<List<LeaderboardRowDto>>> GetLeaderboard(
        string mapKey,
        int difficulty,
        int centerRank,
        int take = 20)
    {
        var query = _db.Scores
            .Include(x => x.Player)
            .Include(x => x.WeaponStats)
            .ThenInclude(ws => ws.Weapon)
            .Where(x => x.Map.MapKey == mapKey && x.Difficulty == difficulty)
            .OrderByDescending(x => x.Value);

        var list = await query.ToListAsync();

        var bestPerPlayer = list
            .GroupBy(s => s.Player.Id)
            .Select(g => g.First())
            .OrderByDescending(s => s.Value)
            .ToList();

        var ranked = bestPerPlayer
            .Select((s, index) => new { Score = s, Rank = index + 1 })
            .ToList();

        int betterCount = (take + 1) / 2;
        int worseCount = take / 2;

        var topRank = centerRank - betterCount;
        var overhead = Math.Min(topRank, 0);

        topRank = Math.Max(0, topRank);
        var botRank = centerRank + worseCount - overhead;

        var window = ranked
            .Where(x => x.Rank > topRank &&
                        x.Rank <= botRank)
            .ToList();

        var result = window.Select(x =>
        {
            var bestWeapon = x.Score.WeaponStats
                .OrderByDescending(w => w.Damage)
                .FirstOrDefault();

            return new LeaderboardRowDto(
                    x.Rank,
                    x.Score.Id,
                    x.Score.Player.DisplayName,
                    x.Score.PlayerId,
                    x.Score.TimeMs,
                    x.Score.Value,
                    x.Score.WeaponStats.Sum(w => w.Kills),
                    x.Score.WeaponStats.Sum(w => w.Damage),
                    bestWeapon?.Weapon.WeaponKey ?? "Unknown",
                    bestWeapon?.Accuracy
                );
        });

        return Ok(result);
    }

    [HttpGet("leaderboard/unique/around-score")]
    public async Task<ActionResult<List<LeaderboardRowDto>>> GetLeaderboardAroundScore(
        string mapKey,
        int difficulty,
        Guid scoreId,
        int take = 20)
    {
        var list = await _db.Scores
            .Include(x => x.Player)
            .Include(x => x.WeaponStats)
            .ThenInclude(ws => ws.Weapon)
            .Where(x => x.Map.MapKey == mapKey && x.Difficulty == difficulty)
            .OrderByDescending(x => x.Value)
            .ToListAsync();

        var targetScore = list.FirstOrDefault(s => s.Id == scoreId);
        if (targetScore == null)
            return NotFound($"Score {scoreId} not found for the given map and difficulty.");

        var bestPerPlayer = list
            .GroupBy(s => s.Player.Id)
            .Select(g => g.First())
            .OrderByDescending(s => s.Value)
            .ToList();

        bool alreadyPresent = bestPerPlayer.Any(s => s.Id == scoreId);
        if (!alreadyPresent)
        {
            var insertIndex = bestPerPlayer.FindIndex(s => s.Value < targetScore.Value);
            if (insertIndex == -1)
                bestPerPlayer.Add(targetScore);
            else
                bestPerPlayer.Insert(insertIndex, targetScore);
        }

        var ranked = bestPerPlayer
            .Select((s, index) => new { Score = s, Rank = index + 1 })
            .ToList();

        int centerRank = ranked.First(x => x.Score.Id == scoreId).Rank;

        int betterCount = (take + 1) / 2;
        int worseCount = take / 2;
        var topRank = centerRank - betterCount;
        var overhead = Math.Min(topRank, 0);
        topRank = Math.Max(0, topRank);
        var botRank = centerRank + worseCount - overhead;

        var window = ranked
            .Where(x => x.Rank > topRank && x.Rank <= botRank)
            .ToList();

        var result = window.Select(x =>
        {
            var bestWeapon = x.Score.WeaponStats
                .OrderByDescending(w => w.Damage)
                .FirstOrDefault();

            return new LeaderboardRowDto(
                x.Rank,
                x.Score.Id,
                x.Score.Player.Username,
                x.Score.PlayerId,
                x.Score.TimeMs,
                x.Score.Value,
                x.Score.WeaponStats.Sum(w => w.Kills),
                x.Score.WeaponStats.Sum(w => w.Damage),
                bestWeapon?.Weapon.WeaponKey ?? "Unknown",
                bestWeapon?.Accuracy
            );
        });

        return Ok(result);
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<LeaderboardRowDto>>> GetLeaderboardUnique(
        string mapKey,
        int difficulty,
        int centerRank,
        int take = 20)
    {
        var query = _db.Scores
            .Include(x => x.Player)
            .Include(x => x.WeaponStats)
            .ThenInclude(ws => ws.Weapon)
            .Where(x => x.Map.MapKey == mapKey && x.Difficulty == difficulty)
            .OrderByDescending(x => x.Value);

        var list = await query.ToListAsync();

        var ranked = list
            .Select((s, index) => new { Score = s, Rank = index + 1 })
            .ToList();

        int betterCount = (take + 1) / 2;
        int worseCount = take / 2;

        var topRank = centerRank - betterCount;
        var overhead = Math.Min(topRank, 0);

        topRank = Math.Max(0, topRank);
        var botRank = centerRank + worseCount - overhead;

        var window = ranked
            .Where(x => x.Rank > topRank &&
                        x.Rank <= botRank)
            .ToList();

        var result = window.Select(x =>
        {
            var bestWeapon = x.Score.WeaponStats
                .OrderByDescending(w => w.Damage)
                .FirstOrDefault();

            return new LeaderboardRowDto(
                    x.Rank,
                    x.Score.Id,
                    x.Score.Player.Username,
                    x.Score.PlayerId,
                    x.Score.TimeMs,
                    x.Score.Value,
                    x.Score.WeaponStats.Sum(w => w.Kills),
                    x.Score.WeaponStats.Sum(w => w.Damage),
                    bestWeapon?.Weapon.WeaponKey ?? "Unknown",
                    bestWeapon?.Accuracy
                );
        });

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScoreDto>> GetScore(Guid id)
    {
        var score = await _db.Scores
            .Include(x => x.Player)
            .Include(x => x.Map)
            .Include(x => x.WeaponStats)
            .ThenInclude(ws => ws.Weapon)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (score == null)
            return NotFound();

        return new ScoreDto(
            score.Id,
            score.Player.Username,
            score.Map.MapKey,
            score.Difficulty,
            score.Value,
            score.TimeMs,
            score.WeaponStats.Select(ws => new WeaponStatDto(
                ws.Weapon.WeaponKey,
                ws.Damage,
                ws.Kills,
                ws.Accuracy,
                ws.CriticalAccuracy
            )).ToList()
        );
    }
}