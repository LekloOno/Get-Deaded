using Shared.Scores;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Threading;

namespace Client.Api.Score;

public class ScoreApi : ApiClient
{
    public async Task<ScoreResult> SubmitAsync(SubmitScoreRequest request, CancellationToken ct = default)
    {
        var result = await SendAsync<SubmitScoreResponse>(HttpMethod.Post, "api/scores", request, ct);
        return ToResult(result);
    }

    public async Task<ApiResult<ScoreDto>> GetDetailAsync(Guid scoreId, CancellationToken ct = default) =>
        await SendAsync<ScoreDto>(HttpMethod.Get, $"api/scores/{scoreId}", ct: ct);

    private static ScoreResult ToResult(ApiResult<SubmitScoreResponse> result)
    {
        if (!result.Success)
        {
            var error = MapError(result);
            return new ScoreResult { Success = false, Error = error, Message = ToMessage(error) };
        }

        return new ScoreResult { Success = true, ScoreId = result.Data!.ScoreId, Rank = result.Data.Rank };
    }

    private static string ToMessage(ApiErrorType error) => error switch
    {
        ApiErrorType.Unauthorized => "You must be logged in to submit a score.",
        ApiErrorType.InvalidRequest => "This score couldn't be validated — try again.",
        ApiErrorType.NetworkError => "Could not reach the server.",
        ApiErrorType.Timeout => "The request timed out.",
        ApiErrorType.ServerError => "Something went wrong on our end.",
        ApiErrorType.RateLimited => "Too many attempts. Please wait a moment before trying again.",
        _ => "An unknown error occurred."
    };
}