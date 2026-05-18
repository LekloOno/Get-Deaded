namespace Client.Auth;


public static class Session
{
    public static Observable<string?> Token { get; set; } = new(null);
    public static bool Offline { get; set; }

    public static bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(Token);

    public static void Logout() =>
        Token.Value = null;
}