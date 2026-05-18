using System.Text;
using System.Text.Json;
using Shared.Scores;
using Client.Api.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System;

namespace Client.Api.Score;

public class ScoreApi : ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public async Task<ScoreResult> SubmitScoreAsync(
        SubmitScoreRequest score)
    {
        try
        {
            var json = JsonSerializer.Serialize(
                score,
                JsonOptions);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/scores")
            {
                Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json")
            };

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", Session.Token);

            var response = await Http.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new ScoreResult
                {
                    Success = false,
                    Error = ScoreErrorType.Unauthorized,
                    Message = "You are not authorized."
                };
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return new ScoreResult
                {
                    Success = false,
                    Error = ScoreErrorType.InvalidRequest,
                    Message = "Invalid score submission."
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                return new ScoreResult
                {
                    Success = false,
                    Error = ScoreErrorType.ServerError,
                    Message = $"Server error: {(int)response.StatusCode}"
                };
            }


            return new ScoreResult
            {
                Success = true,
                Error = ScoreErrorType.None
            };
        }
        catch (TaskCanceledException)
        {
            return new ScoreResult
            {
                Success = false,
                Error = ScoreErrorType.Timeout,
                Message = "Request timed out."
            };
        }
        catch (HttpRequestException)
        {
            return new ScoreResult
            {
                Success = false,
                Error = ScoreErrorType.NetworkError,
                Message = "Cannot reach server."
            };
        }
        catch (Exception ex)
        {
            return new ScoreResult
            {
                Success = false,
                Error = ScoreErrorType.Unknown,
                Message = ex.Message
            };
        }
    }
}