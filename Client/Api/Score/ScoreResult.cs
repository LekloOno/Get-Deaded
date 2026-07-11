using System;

namespace Client.Api.Score;

public class ScoreResult
{
    public bool Success { get; set; }
    public ApiErrorType Error { get; set; }
    public string? Message { get; set; }

    public Guid? ScoreId { get; set; }
    public int? Rank { get; set; }
}