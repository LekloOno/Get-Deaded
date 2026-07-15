using Data.Entities;

namespace Api.Version;

public interface IGameVersionResolver
{
    Task<GameVersion?> ResolveAsync(string versionKey);
    Task<ScoreCompatibilityGroup?> ResolveCompatibilityGroupAsync(string versionKey);
}