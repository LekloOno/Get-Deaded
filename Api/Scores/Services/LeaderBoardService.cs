using Api.Scores.Queries;
using Api.Version;
using Data.Db;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Scores;

namespace Api.Scores.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly GameDbContext _db;
    private readonly GameVersionContext _versionContext;
    private readonly LeaderboardQueries _queries;

    public LeaderboardService(GameDbContext db, GameVersionContext versionContext, LeaderboardQueries queries)
    {
        _db = db;
        _versionContext = versionContext;
        _queries = queries;
    }

    public Task<int> GetRankAsync(LeaderboardScope scope, Guid playerId, int value, CancellationToken ct) =>
        _queries.GetRankAsync(scope.MapKey, scope.ModeKey, _versionContext.Version, scope.Difficulty, playerId, value, ct);

    public async Task<List<LeaderboardRowDto>> GetWindowAsync(
        LeaderboardScope scope, int centerRank, int take, CancellationToken ct) =>
        await BuildWindowAsync(scope, centerRank, take, highlightScoreId: null, ct);

    private async Task<List<LeaderboardRowDto>> BuildWindowAsync(
        LeaderboardScope scope, int centerRank, int take, Guid? highlightScoreId, CancellationToken ct)
    {
        var (topRank, botRank) = ComputeWindow(centerRank, take);
        var rows = await _queries.GetRankedWindowAsync(scope.MapKey, scope.ModeKey, _versionContext.Version, scope.Difficulty, topRank, botRank, ct);
        return await HydrateAsync(rows, highlightScoreId, ct);
    }

    public async Task<List<LeaderboardRowDto>> GetAroundScoreAsync(Guid scoreId, int take, CancellationToken ct)
    {
        var target = await _db.Scores
            .AsNoTracking()
            .Where(s => s.Id == scoreId)
            .Select(s => new { s.PlayerId, s.Value, s.MapKey, s.ModeKey, s.Difficulty })
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"Score {scoreId} not found.");

        var scope = new LeaderboardScope(target.MapKey, target.ModeKey, (Difficulty) target.Difficulty);
        var rank = await GetRankAsync(scope, target.PlayerId, target.Value, ct);
        var window = await BuildWindowAsync(scope, rank, take, highlightScoreId: scoreId, ct);

        if (window.Any(r => r.ScoreId == scoreId))
            return window;

        var submitted = await BuildRowAsync(scoreId, rank, submitted: true, isPersonalBest: false, ct);
        return [.. window.Append(submitted).OrderBy(r => r.Rank)];
    }

    private static (int TopRank, int BotRank) ComputeWindow(int centerRank, int take)
    {
        int betterCount = (take + 1) / 2;
        int worseCount = take / 2;

        var topRank = centerRank - betterCount;
        var overhead = Math.Min(topRank, 0);
        topRank = Math.Max(0, topRank);
        var botRank = centerRank + worseCount - overhead;

        return (topRank, botRank);
    }

    private static LeaderboardRowDto ToDto(Score s, int rank, bool submitted, bool isPersonalBest)
    {
        var bestWeapon = s.WeaponStats.OrderByDescending(w => w.Damage).FirstOrDefault();
        return new LeaderboardRowDto(
            rank, s.Id, s.Player.DisplayName, s.PlayerId, s.TimeMs, s.Value,
            s.WeaponStats.Sum(w => w.Kills),
            s.WeaponStats.Sum(w => w.Damage),
            bestWeapon?.Weapon.WeaponKey ?? "Unknown",
            bestWeapon?.Accuracy,
            submitted, isPersonalBest);
    }

    private async Task<List<LeaderboardRowDto>> HydrateAsync(
        List<RankedScoreRow> rows, Guid? highlightScoreId, CancellationToken ct)
    {
        if (rows.Count == 0) return [];

        var ids = rows.Select(r => r.Id).ToList();
        var scores = await _db.Scores
            .AsNoTracking()
            .Include(s => s.Player)
            .Include(s => s.WeaponStats).ThenInclude(ws => ws.Weapon)
            .Where(s => ids.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, ct);

        return [.. rows.Select(r => ToDto(
            scores[r.Id], r.Rank,
            submitted: r.Id == highlightScoreId,
            isPersonalBest: true))];
    }

    private async Task<LeaderboardRowDto> BuildRowAsync(Guid scoreId, int rank, bool submitted, bool isPersonalBest, CancellationToken ct)
    {
        var s = await _db.Scores
            .AsNoTracking()
            .Include(x => x.Player)
            .Include(x => x.WeaponStats).ThenInclude(ws => ws.Weapon)
            .FirstAsync(x => x.Id == scoreId, ct);

        return ToDto(s, rank, submitted, isPersonalBest);
    }
}