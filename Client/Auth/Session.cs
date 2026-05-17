namespace Client.Auth;


public static class Session
{
    public static string? Token { get; set; }

    public static bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(Token);
}