using Data.Db;
using Microsoft.EntityFrameworkCore;
using Shared.Scores;

namespace Api.Scores;

public record RankedScoreRow(
    int Rank,
    Guid Id,
    Guid PlayerId,
    string PlayerUsername,
    int TimeMs,
    int Value
);

public static class LeaderboardQueries
{
    public static async Task<List<LeaderboardRowDto<T>>> GetCenteredLeaderboard<T>(
        GameDbContext db,
        string mapKey,
        int difficulty,
        int centerRank,
        int take,
        string modeDetailTable, // hardcoded literal per mode, e.g. "ArenaScoreDetails" -- NEVER from request input
        Func<GameDbContext, IReadOnlyCollection<Guid>, Task<Dictionary<Guid, T>>> loadModeDetails)
    {
        int betterCount = (take + 1) / 2;
        int worseCount = take / 2;

        var topRank = centerRank - betterCount;
        var overhead = Math.Min(topRank, 0);
        topRank = Math.Max(0, topRank);
        var botRank = centerRank + worseCount - overhead;

        var sql = $"""
            WITH best_per_player AS (
                SELECT DISTINCT ON (s."PlayerId")
                    s."Id"        AS "Id",
                    s."PlayerId"  AS "PlayerId",
                    p."Username"  AS "PlayerUsername",
                    s."TimeMs"    AS "TimeMs",
                    s."Value"     AS "Value"
                FROM "scores" s
                JOIN "players" p ON p."Id" = s."PlayerId"
                JOIN "{modeDetailTable}" md ON md."ScoreId" = s."Id"
                WHERE s."MapKey" = @p0
                  AND s."Difficulty" = @p1
                ORDER BY s."PlayerId", s."Value" DESC, s."Id"
            ),
            ranked AS (
                SELECT *, (ROW_NUMBER() OVER (ORDER BY "Value" DESC, "Id"))::int AS "Rank"
                FROM best_per_player
            )
            SELECT * FROM ranked
            WHERE "Rank" > @p2 AND "Rank" <= @p3
            ORDER BY "Rank"
            """;

        var rows = await db.Database
            .SqlQueryRaw<RankedScoreRow>(sql, mapKey, difficulty, topRank, botRank)
            .ToListAsync();

        if (rows.Count == 0)
            return [];

        var scoreIds = rows.Select(r => r.Id).ToList();

        var weaponLookup = (await db.WeaponStats
                .Include(w => w.Weapon)
                .Where(w => scoreIds.Contains(w.ScoreId))
                .ToListAsync())
            .GroupBy(w => w.ScoreId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var modeDetails = await loadModeDetails(db, scoreIds);

        return [.. rows.Select(r =>
        {
            var weapons = weaponLookup.GetValueOrDefault(r.Id, []);
            var bestWeapon = weapons.OrderByDescending(w => w.Damage).FirstOrDefault();

            return new LeaderboardRowDto<T>(
                r.Rank, r.Id, r.PlayerUsername, r.PlayerId, r.TimeMs, r.Value,
                weapons.Sum(w => w.Kills),
                weapons.Sum(w => w.Damage),
                bestWeapon?.Weapon.WeaponKey ?? "Unknown",
                bestWeapon?.Accuracy,
                modeDetails[r.Id]
            );
        })];
    }
}