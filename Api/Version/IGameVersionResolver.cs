using Data.Entities;

namespace Api.Version;

public interface IGameVersionResolver
{
    Task<GameVersion?> ResolveAsync(string versionString);
    Task<ScoreCompatibilityGroup?> ResolveCompatibilityGroupAsync(string versionString);
}