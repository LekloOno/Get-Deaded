using System.Text;
using System.Text.Json;
using Shared.Auth;
using Client.Auth;
using System.Threading.Tasks;
using System.Net.Http;

namespace Client.Api;

public class AuthApi : ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public async Task<bool> RegisterAsync(
        string username,
        string password)
    {
        var req = new RegisterRequest(
            username,
            password);

        var json = JsonSerializer.Serialize(req, JsonOptions);

        var response = await Http.PostAsync(
            "api/auth/register",
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
            return false;

        var body = await response.Content.ReadAsStringAsync();

        var auth = JsonSerializer.Deserialize<AuthResponse>(
            body,
            JsonOptions)!;

        Session.Token.Value = auth.Token;

        return true;
    }

    public async Task<bool> LoginAsync(
        string username,
        string password)
    {
        var req = new LoginRequest(
            username,
            password);

        var json = JsonSerializer.Serialize(req, JsonOptions);

        var response = await Http.PostAsync(
            "api/auth/login",
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
            return false;

        var body = await response.Content.ReadAsStringAsync();

        var auth = JsonSerializer.Deserialize<AuthResponse>(
            body,
            JsonOptions)!;

        Session.Token.Value = auth.Token;

        return true;
    }
}