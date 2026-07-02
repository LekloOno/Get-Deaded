using System.Text.Json;
using Shared.Scores;
using Client.Api.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using Godot;

namespace Client.Api.Score;

public partial class ScoreApi : ApiClient
{
    public async Task<ApiResult<List<LeaderboardRowDto>>> GetLeaderboardAsync(
    string mapKey,
    int difficulty,
    int centerRank,
    int take = 20)
    {
        var result = new ApiResult<List<LeaderboardRowDto>>();

        try
        {
            var url =
                $"api/scores/leaderboard/unique?mapKey={mapKey}&difficulty={difficulty}&centerRank={centerRank}&take={take}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", Session.Token);

            request.Headers.Add("X-Client-Version", Session.Version);

            var response = await Http.SendAsync(request);

            result.StatusCode = (int)response.StatusCode;

            var body = await response.Content.ReadAsStringAsync();
            result.RawBody = body;

            if (!response.IsSuccessStatusCode)
            {
                result.Success = false;
                result.ErrorMessage = $"HTTP {(int)response.StatusCode}";
                return result;
            }

            result.Data = JsonSerializer.Deserialize<List<LeaderboardRowDto>>(body, JsonOptions);
            result.Success = true;
            return result;
        }
        catch (TaskCanceledException ex)
        {
            result.Success = false;
            result.ExceptionType = nameof(TaskCanceledException);
            result.ErrorMessage = "Timeout";
            GD.PrintErr(ex);
            return result;
        }
        catch (HttpRequestException ex)
        {
            result.Success = false;
            result.ExceptionType = nameof(HttpRequestException);
            result.ErrorMessage = ex.Message;
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ExceptionType = ex.GetType().Name;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public async Task<ApiResult<List<LeaderboardRowDto>>> GetLeaderboardNewScoreAsync(
    string mapKey,
    int difficulty,
    Guid scoreId,
    int take = 20)
    {
        var result = new ApiResult<List<LeaderboardRowDto>>();

        try
        {
            var url =
                $"api/scores/leaderboard/unique/around-score?mapKey={mapKey}&difficulty={difficulty}&scoreId={scoreId}&take={take}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", Session.Token);

            request.Headers.Add("X-Client-Version", Session.Version);

            var response = await Http.SendAsync(request);

            result.StatusCode = (int)response.StatusCode;

            var body = await response.Content.ReadAsStringAsync();
            result.RawBody = body;

            if (!response.IsSuccessStatusCode)
            {
                result.Success = false;
                result.ErrorMessage = $"HTTP {(int)response.StatusCode}";
                return result;
            }

            result.Data = JsonSerializer.Deserialize<List<LeaderboardRowDto>>(body, JsonOptions);
            result.Success = true;
            return result;
        }
        catch (TaskCanceledException ex)
        {
            result.Success = false;
            result.ExceptionType = nameof(TaskCanceledException);
            result.ErrorMessage = "Timeout";
            GD.PrintErr(ex);
            return result;
        }
        catch (HttpRequestException ex)
        {
            result.Success = false;
            result.ExceptionType = nameof(HttpRequestException);
            result.ErrorMessage = ex.Message;
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ExceptionType = ex.GetType().Name;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public async Task<ApiResult<ScoreDto>> GetScoreDetailAsync(Guid scoreId)
    {
        var result = new ApiResult<ScoreDto>();

        try
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/scores/{scoreId}"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", Session.Token);

            request.Headers.Add("X-Client-Version", Session.Version);

            var response = await Http.SendAsync(request);

            result.StatusCode = (int)response.StatusCode;

            var body = await response.Content.ReadAsStringAsync();
            result.RawBody = body;

            if (!response.IsSuccessStatusCode)
            {
                result.Success = false;
                result.ErrorMessage = $"HTTP {(int)response.StatusCode}";
                return result;
            }

            result.Data = JsonSerializer.Deserialize<ScoreDto>(body, JsonOptions);
            result.Success = true;
            return result;
        }
        catch (TaskCanceledException ex)
        {
            result.Success = false;
            result.ExceptionType = nameof(TaskCanceledException);
            result.ErrorMessage = "Timeout";
            GD.PrintErr(ex);
            return result;
        }
        catch (HttpRequestException ex)
        {
            result.Success = false;
            result.ExceptionType = nameof(HttpRequestException);
            result.ErrorMessage = ex.Message;
            GD.PrintErr(ex);
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ExceptionType = ex.GetType().Name;
            result.ErrorMessage = ex.Message;
            GD.PrintErr(ex);
            return result;
        }
    }
}