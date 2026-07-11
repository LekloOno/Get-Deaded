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
            return new AuthResult { Success = false, Error = (AuthErrorType)MapError(result), Message = result.ErrorMessage };

        Session.Token.Value = result.Data!.Token;
        Session.PlayerId = result.Data.UserId;

        return new AuthResult { Success = true, Token = result.Data.Token };
    }
}