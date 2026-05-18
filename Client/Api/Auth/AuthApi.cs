using System.Text;
using System.Text.Json;
using Shared.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System;

namespace Client.Api.Auth;

public class AuthApi : ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public async Task<AuthResult> RegisterAsync(
        string username,
        string password)
    {
        try
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

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return new AuthResult
                {
                    Success = false,
                    Error = AuthErrorType.InvalidCredentials,
                    Message = "Invalid username or password."
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                return new AuthResult
                {
                    Success = false,
                    Error = AuthErrorType.ServerError,
                    Message = $"Server error: {(int)response.StatusCode}"
                };
            }

            var body = await response.Content.ReadAsStringAsync();

            var auth = JsonSerializer.Deserialize<AuthResponse>(
                body,
                JsonOptions)!;

            Session.Token.Value = auth.Token;

            return new AuthResult
            {
                Success = true,
                Token = auth.Token
            };
        }
        catch (TaskCanceledException)
        {
            return new AuthResult
            {
                Success = false,
                Error = AuthErrorType.Timeout,
                Message = "Request timed out."
            };
        }
        catch (HttpRequestException)
        {
            return new AuthResult
            {
                Success = false,
                Error = AuthErrorType.NetworkError,
                Message = "Cannot reach server."
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Error = AuthErrorType.Unknown,
                Message = ex.Message
            };
        }
    }

    public async Task<AuthResult> LoginAsync(
        string username,
        string password)
    {
        try
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

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return new AuthResult
                {
                    Success = false,
                    Error = AuthErrorType.InvalidCredentials,
                    Message = "Invalid username or password."
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                return new AuthResult
                {
                    Success = false,
                    Error = AuthErrorType.ServerError,
                    Message = $"Server error: {(int)response.StatusCode}"
                };
            }

            var body = await response.Content.ReadAsStringAsync();

            var auth = JsonSerializer.Deserialize<AuthResponse>(
                body,
                JsonOptions)!;

            if (auth == null || string.IsNullOrEmpty(auth.Token))
            {
                return new AuthResult
                {
                    Success = false,
                    Error = AuthErrorType.ServerError,
                    Message = "Invalid server response."
                };
            }

            Session.Token.Value = auth.Token;

            return new AuthResult
            {
                Success = true,
                Token = auth.Token,
                Error = AuthErrorType.None
            };
        }
        catch (TaskCanceledException)
        {
            return new AuthResult
            {
                Success = false,
                Error = AuthErrorType.Timeout,
                Message = "Request timed out."
            };
        }
        catch (HttpRequestException)
        {
            return new AuthResult
            {
                Success = false,
                Error = AuthErrorType.NetworkError,
                Message = "Cannot reach server."
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Error = AuthErrorType.Unknown,
                Message = ex.Message
            };
        }
    }
}