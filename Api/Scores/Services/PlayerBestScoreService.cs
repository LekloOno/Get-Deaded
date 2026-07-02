using Data.Db;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Scores;

public class PlayerBestScoreService
{
    private readonly GameDbContext _db;

    public PlayerBestScoreService(GameDbContext db)
    {
        _db = db;
    }

    public async Task UpsertIfBetterAsync(Score score)
    {
        var best = await _db.Set<PlayerBestScore>()
            .SingleOrDefaultAsync(x =>
                x.PlayerId == score.PlayerId &&
                x.MapKey == score.MapKey &&
                x.Difficulty == score.Difficulty &&
                x.ModeKey == score.ModeKey);

        if (best == null)
        {
            _db.Add(new PlayerBestScore
            {
                PlayerId = score.PlayerId,
                MapKey = score.MapKey,
                Difficulty = score.Difficulty,
                ModeKey = score.ModeKey,
                ScoreId = score.Id,
                Value = score.Value
            });

            return;
        }

        if (score.Value <= best.Value)
            return;

        best.ScoreId = score.Id;
        best.Value = score.Value;
    }
}