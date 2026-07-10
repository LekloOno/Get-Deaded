namespace Shared.Auth;

public record LoginRequest(
    string Username,
    string Password
);

public record AuthResponse(
    string Token,
    Guid UserId,
    string Username,
    string DisplayName
);

public record RegisterRequest(
    string Username,
    string Password
);