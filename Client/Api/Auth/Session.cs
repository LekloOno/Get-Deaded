using System;

namespace Client.Api.Auth;


public static class Session
{
    public const string Version = "0.2.4";
    public static Observable<string?> Token { get; set; } = new(null);
    public static bool Offline { get; set; }
    public static Guid? PlayerId { get; set; }

    public static bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(Token);

    public static void Logout() =>
        Token.Value = null;
}