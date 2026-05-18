namespace Client.Api.Score;

// Quite redundant, gotta figure out a way to abstract api errors from domain specific errors later
public enum ScoreErrorType
{
    None,
    Unauthorized,
    InvalidRequest,
    ServerError,
    NetworkError,
    Timeout,
    Unknown
}

public class ScoreResult
{
    public bool Success { get; set; }
    public ScoreErrorType Error { get; set; }
    public string? Message { get; set; }
}