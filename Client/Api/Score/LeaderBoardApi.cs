using System.Text.Json;
using Shared.Scores;
using Client.Api.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace Client.Api.Score;

public partial class ScoreApi : ApiClient
{
    public async Task<List<LeaderboardRowDto>?> GetLeaderboardAsync(
    string mapKey,
    int difficulty,
    int centerRank,
    int take = 20)
    {
        try
        {
            var url =
                $"api/scores/leaderboard?mapKey={mapKey}&difficulty={difficulty}&centerRank={centerRank}&take={take}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", Session.Token);

            var response = await Http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<LeaderboardRowDto>>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ScoreDto?> GetScoreDetailAsync(Guid scoreId)
    {
        try
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/scores/{scoreId}"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", Session.Token);

            var response = await Http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ScoreDto>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }
}