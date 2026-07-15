using Api.Version;
using Data.Db;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Scores;

namespace Api.Scores.Services;

public class ScoreService : IScoreService
{
    private readonly GameDbContext _db;
    private readonly GameVersionContext _versionContext;
    private readonly ILeaderboardService _leaderboard;

    public ScoreService(GameDbContext db, GameVersionContext versionContext, ILeaderboardService leaderboard)
    {
        _db = db;
        _versionContext = versionContext;
        _leaderboard = leaderboard;
    }

    public async Task<SubmitScoreResponse> SubmitAsync(Guid playerId, SubmitScoreRequest request, CancellationToken ct)
    {
        if (!await _db.Maps.AnyAsync(m => m.MapKey == request.MapKey, ct))
            throw new ArgumentException($"Unknown map '{request.MapKey}'.");

        var score = new Score
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
            MapKey = request.MapKey,
            ModeKey = request.ModeKey,
            Difficulty = (int) request.Difficulty,
            VersionString = _versionContext.Version!.VersionString,
            TimeMs = request.TimeMs,
            Value = request.Value,
            WeaponStats = [.. request.WeaponStats.Select(ws => new WeaponStat
            {
                Id = Guid.NewGuid(),
                WeaponKey = ws.WeaponKey,
                Damage = ws.Damage,
                Kills = ws.Kills,
                Accuracy = ws.Accuracy,
                CriticalAccuracy = ws.CriticalAccuracy
            })]
        };

        _db.Scores.Add(score);
        await _db.SaveChangesAsync(ct);

        var scope = new LeaderboardScope(request.MapKey, request.ModeKey, request.Difficulty);
        var rank = await _leaderboard.GetRankAsync(scope, playerId, request.Value, ct);

        return new SubmitScoreResponse(score.Id, rank);
    }

    public async Task<ScoreDto?> GetDetailAsync(Guid scoreId, CancellationToken ct)
    {
        var score = await _db.Scores
            .AsNoTracking()
            .Include(s => s.Player)
            .Include(s => s.WeaponStats).ThenInclude(ws => ws.Weapon)
            .FirstOrDefaultAsync(s => s.Id == scoreId, ct);

        if (score is null) return null;

        return new ScoreDto(
            score.Id, score.Player.DisplayName,
            score.MapKey, score.ModeKey, (Difficulty) score.Difficulty,
            score.Value, score.TimeMs,
            [.. score.WeaponStats.Select(ws => new WeaponStatDto(
                ws.WeaponKey, ws.Damage, ws.Kills, ws.Accuracy, ws.CriticalAccuracy))]);
    }
}