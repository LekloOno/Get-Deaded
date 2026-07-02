using Data.Entities;

namespace Api.Context;

public sealed class GameSession
{
    public Player? Player { get; set; }
    public ClientVersion Version { get; set; } = null!;

    public bool IsAuthenticated => Player is not null;
}