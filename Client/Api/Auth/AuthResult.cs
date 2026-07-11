namespace Client.Api.Auth;

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public ApiErrorType Error { get; set; }
    public string? Message { get; set; }
}