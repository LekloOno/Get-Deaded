namespace Client.Api;

public enum ApiErrorType
{
    None,
    Unauthorized,
    InvalidRequest,
    Conflict,
    ServerError,
    NetworkError,
    Timeout,
    Unknown
}