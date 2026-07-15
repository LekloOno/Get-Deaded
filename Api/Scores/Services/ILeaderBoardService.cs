using Shared.Scores;

namespace Api.Scores.Services;

public record LeaderboardScope(string MapKey, string ModeKey, Difficulty Difficulty);

public interface ILeaderboardService
{
    Task<int> GetRankAsync(LeaderboardScope scope, Guid playerId, int value, CancellationToken ct);
    Task<List<LeaderboardRowDto>> GetWindowAsync(LeaderboardScope scope, int centerRank, int take, CancellationToken ct);
    Task<List<LeaderboardRowDto>> GetAroundScoreAsync(Guid scoreId, int take, CancellationToken ct);
}