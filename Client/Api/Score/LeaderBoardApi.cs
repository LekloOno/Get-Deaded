using Shared.Scores;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Client.Api.Score;

public class LeaderboardApi : ApiClient
{
    public async Task<ApiResult<List<LeaderboardRowDto>>> GetWindowAsync(
        string mapKey, int difficulty, int centerRank, int take, CancellationToken ct = default) =>
        await SendAsync<List<LeaderboardRowDto>>(HttpMethod.Get,
            $"api/leaderboard?mapKey={mapKey}&difficulty={difficulty}&centerRank={centerRank}&take={take}", ct: ct);

    public async Task<ApiResult<List<LeaderboardRowDto>>> GetAroundScoreAsync(
        Guid scoreId, int take, CancellationToken ct = default) =>
        await SendAsync<List<LeaderboardRowDto>>(HttpMethod.Get,
            $"api/leaderboard/around/{scoreId}?take={take}", ct: ct);
}