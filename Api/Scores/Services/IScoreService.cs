using Shared.Scores;

namespace Api.Scores.Services;

public interface IScoreService
{
    Task<SubmitScoreResponse> SubmitAsync(Guid playerId, SubmitScoreRequest request, CancellationToken ct);
    Task<ScoreDto?> GetDetailAsync(Guid scoreId, CancellationToken ct);
}