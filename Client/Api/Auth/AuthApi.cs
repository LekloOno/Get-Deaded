using Shared.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace Client.Api.Auth;

public class AuthApi : ApiClient
{
    public async Task<AuthResult> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var result = await SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/login",
            new LoginRequest(username, password), ct);
        return ToResult(result);
    }

    public async Task<AuthResult> RegisterAsync(string username, string password, CancellationToken ct = default)
    {
        var result = await SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/register",
            new RegisterRequest(username, password), ct);
        return ToResult(result);
    }

    private static AuthResult ToResult(ApiResult<AuthResponse> result)
    {
        if (!result.Success)
        {
            var error = MapError(result);
            return new AuthResult { Success = false, Error = error, Message = ToMessage(error) };
        }

        Session.Token.Value = result.Data!.Token;
        Session.PlayerId = result.Data.UserId;
        return new AuthResult { Success = true, Token = result.Data.Token };
    }

    private static string ToMessage(ApiErrorType error) => error switch
    {
        ApiErrorType.Unauthorized => "Invalid username or password.",
        ApiErrorType.Conflict => "This username is already taken.",
        ApiErrorType.InvalidRequest => "Invalid request.",
        ApiErrorType.NetworkError => "Could not reach the server.",
        ApiErrorType.Timeout => "The request timed out.",
        ApiErrorType.ServerError => "Something went wrong on our end.",
        ApiErrorType.RateLimited => "Too many attempts. Please wait a moment before trying again.",
        _ => "An unknown error occurred."
    };
}