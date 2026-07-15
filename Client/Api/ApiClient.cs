using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Api;

public abstract class ApiClient
{
    private static readonly HttpClient _http =
        new(new AuthHeaderHandler
        {
            InnerHandler = new GameVersionHeaderHandler
            {
                InnerHandler = new HttpClientHandler()
            }
        })
    {
        BaseAddress = new Uri(ApiConfig.BaseUrl)
    };

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method, string url, object? body = null, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (body is not null)
                request.Content = JsonContent.Create(body, options: JsonOptions);

            using var response = await _http.SendAsync(request, ct);
            var raw = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<T>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    RawBody = raw,
                    ErrorMessage = raw
                };
            }

            var data = JsonSerializer.Deserialize<T>(raw, JsonOptions);
            return new ApiResult<T> { Success = true, Data = data, StatusCode = (int)response.StatusCode, RawBody = raw };
        }
        catch (OperationCanceledException)
        {
            return new ApiResult<T> { Success = false, ExceptionType = nameof(OperationCanceledException) };
        }
        catch (HttpRequestException ex)
        {
            return new ApiResult<T> { Success = false, ExceptionType = nameof(HttpRequestException), ErrorMessage = ex.Message };
        }
        catch (Exception ex)
        {
            return new ApiResult<T> { Success = false, ExceptionType = ex.GetType().Name, ErrorMessage = ex.Message };
        }
    }

    protected static ApiErrorType MapError<T>(ApiResult<T> result)
    {
        if (result.ExceptionType == nameof(OperationCanceledException)) return ApiErrorType.Timeout;
        if (result.ExceptionType is not null) return ApiErrorType.NetworkError;

        return result.StatusCode switch
        {
            401 => ApiErrorType.Unauthorized,
            409 => ApiErrorType.Conflict,
            429 => ApiErrorType.RateLimited,
            400 or 422 => ApiErrorType.InvalidRequest,
            >= 500 => ApiErrorType.ServerError,
            _ => ApiErrorType.Unknown
        };
    }
}