namespace Shared.Auth;

public record LoginRequest(
    string Username,
    string Password
);

public record AuthResponse(
    string Token,
    string Username
);

public record RegisterRequest(
    string Username,
    string Password
);