namespace Shared.Scores;

public record SubmitScoreRequest(
    string MapKey,
    int Difficulty,
    int TimeMs,
    int Score,
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

public record LeaderboardRowDto(
    int Rank,
    Guid ScoreId,
    string Player,
    int Score,
    int Kills,
    float Damage,
    string BestWeaponKey,
    float? BestWeaponAccuracy
);

public record LeaderboardPageDto(
    int StartRank,
    int EndRank,
    List<LeaderboardRowDto> Entries
);

public record SubmitScoreResponse(
    Guid ScoreId,
    int Rank
);