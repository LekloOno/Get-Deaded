using System.Text;
using System.Text.Json;
using Shared.Scores;
using Client.Auth;
using System.Threading.Tasks;
using System.Net.Http;

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
        ApplyAuthToken(Session.Token);

        var json = JsonSerializer.Serialize(
            score,
            JsonOptions);

        var response = await Http.PostAsync(
            "api/scores",
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        return response.IsSuccessStatusCode;
    }
}