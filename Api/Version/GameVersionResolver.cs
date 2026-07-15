using Data.Db;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Version;

public class GameVersionResolver : IGameVersionResolver
{
    private readonly GameDbContext _db;

    public GameVersionResolver(GameDbContext db)
    {
        _db = db;
    }

    public Task<GameVersion?> ResolveAsync(string versionKey)
    {
        return _db.GameVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                v => v.VersionKey == versionKey);
    }

    public async Task<ScoreCompatibilityGroup?> ResolveCompatibilityGroupAsync(string versionKey)
    {
        return await _db.GameVersions
            .AsNoTracking()
            .Where(v => v.VersionKey == versionKey)
            .Select(v => v.Group)
            .FirstOrDefaultAsync();
    }
}