namespace Client.Api.Auth;

public enum AuthErrorType
{
    None,
    InvalidCredentials,
    ServerError,
    NetworkError,
    Timeout,
    Unknown
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public AuthErrorType Error { get; set; }
    public string? Message { get; set; }
}