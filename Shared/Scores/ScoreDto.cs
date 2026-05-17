namespace Shared.Scores;

public record SubmitScoreRequest(
    string MapKey,
    int Difficulty,
    int TimeSpentMs,
    int TotalScore,
    List<WeaponStatDto> WeaponStats
);

public record ScoreDto(
    Guid Id,
    string Player,
    string Map,
    int Difficulty,
    int TotalScore,
    int TimeSpentMs,
    List<WeaponStatDto> WeaponStats
);