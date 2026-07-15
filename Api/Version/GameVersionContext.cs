using Data.Entities;

namespace Api.Version;

public class GameVersionContext
{
    public GameVersion Version { get; private set; } = null!;

    public void Set(
        GameVersion version)
    {
        Version = version;
    }
}