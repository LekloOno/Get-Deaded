using Data.Db;
using Microsoft.EntityFrameworkCore;
using Shared.Scores;

namespace Api.Scores.Queries;

public record RankedScoreRow(Guid Id, Guid PlayerId, int Rank, int Value, int TimeMs);

public class LeaderboardQueries
{
    private readonly GameDbContext _db;
    public LeaderboardQueries(GameDbContext db) => _db = db;

    public async Task<int> GetRankAsync(string mapKey, Difficulty difficulty, Guid playerId, int value, CancellationToken ct)
    {
        var rows = await _db.Database.SqlQueryRaw<int>(@"
            WITH best_per_player AS (
                SELECT DISTINCT ON (s.player_id)
                    s.player_id, s.value
                FROM scores s
                WHERE s.map_key = {0} AND s.difficulty = {1}
                ORDER BY s.player_id, s.value DESC, s.id
            )
            SELECT 1 + COUNT(*) AS rank
            FROM best_per_player
            WHERE player_id != {2} AND value > {3}
        ", mapKey, (int)difficulty, playerId, value)
        .ToListAsync(ct);

        return rows[0];
    }

    public Task<List<RankedScoreRow>> GetRankedWindowAsync(
        string mapKey, Difficulty difficulty, int topRank, int botRank, CancellationToken ct)
    {
        return _db.Database.SqlQueryRaw<RankedScoreRow>(@"
            WITH best_per_player AS (
                SELECT DISTINCT ON (s.player_id)
                    s.id, s.player_id, s.value, s.time_ms
                FROM scores s
                WHERE s.map_key = {0} AND s.difficulty = {1}
                ORDER BY s.player_id, s.value DESC, s.id
            ),
            ranked AS (
                SELECT *, ROW_NUMBER() OVER (ORDER BY value DESC, id) AS rank
                FROM best_per_player
            )
            SELECT id, player_id, rank, value, time_ms
            FROM ranked
            WHERE rank > {2} AND rank <= {3}
            ORDER BY rank
        ", mapKey, (int)difficulty, topRank, botRank)
        .ToListAsync(ct);
    }
}