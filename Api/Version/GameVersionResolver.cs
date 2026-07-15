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

    public Task<GameVersion?> ResolveAsync(string versionString)
    {
        return _db.GameVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                v => v.VersionString == versionString);
    }

    public async Task<ScoreCompatibilityGroup?> ResolveCompatibilityGroupAsync(string versionString)
    {
        return await _db.GameVersions
            .AsNoTracking()
            .Where(v => v.VersionString == versionString)
            .Select(v => v.Group)
            .FirstOrDefaultAsync();
    }
}