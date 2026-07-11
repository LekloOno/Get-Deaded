namespace Client.Api;

public enum ApiErrorType
{
    None,
    Unauthorized,
    InvalidRequest,
    Conflict,
    RateLimited,
    ServerError,
    NetworkError,
    Timeout,
    Unknown
}