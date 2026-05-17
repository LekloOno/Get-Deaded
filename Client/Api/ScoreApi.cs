using System.Text;
using System.Text.Json;
using Shared.Scores;
using Client.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Client.Api;

public class ScoreApi : ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public async Task<bool> SubmitScoreAsync(
        SubmitScoreRequest score)
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

        return response.IsSuccessStatusCode;
    }
}