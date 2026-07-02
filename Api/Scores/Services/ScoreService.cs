using Data.Db;
using Data.Entities;

namespace Api.Scores.Services;

public class ScoreService
{
    private readonly GameDbContext _db;
    private readonly PlayerBestScoreService _bestScoreService;

    public ScoreService(
        GameDbContext db,
        PlayerBestScoreService bestScoreService)
    {
        _db = db;
        _bestScoreService = bestScoreService;
    }

    public async Task<Score> SubmitScoreAsync(
        Player player,
        ClientVersion version,
        string mapKey,
        int difficulty,
        int timeMs,
        int value,
        List<WeaponStat> weaponStats,
        string modeKey,
        object modeDetails)
    {
        var score = new Score
        {
            Id = Guid.NewGuid(),
            PlayerId = player.Id,
            MapKey = mapKey,
            Difficulty = difficulty,
            TimeMs = timeMs,
            Value = value,
            ModeKey = modeKey,
            ClientVersionKey = version.VersionKey,
            WeaponStats = weaponStats,
            CreatedAt = DateTime.UtcNow
        };

        _db.Scores.Add(score);

        await _db.SaveChangesAsync();

        await _bestScoreService.UpsertIfBetterAsync(score);

        await _db.SaveChangesAsync();

        return score;
    }
}