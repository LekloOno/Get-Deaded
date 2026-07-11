using System.Text;
using System.Text.Json;
using Shared.Scores;
using Client.Api.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Threading;

namespace Client.Api.Score;

public class ScoreApi : ApiClient
{
    public async Task<ScoreResult> SubmitAsync(SubmitScoreRequest request, CancellationToken ct = default)
    {
        var result = await SendAsync<SubmitScoreResponse>(HttpMethod.Post, "api/scores", request, ct);
        return result.Success
            ? new ScoreResult { Success = true, ScoreId = result.Data!.ScoreId, Rank = result.Data.Rank }
            : new ScoreResult { Success = false, Error = (ScoreErrorType)MapError(result), Message = result.ErrorMessage };
    }

    public async Task<ApiResult<ScoreDto>> GetDetailAsync(Guid scoreId, CancellationToken ct = default) =>
        await SendAsync<ScoreDto>(HttpMethod.Get, $"api/scores/{scoreId}", ct: ct);
}