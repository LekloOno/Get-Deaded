namespace Shared.Scores;

public record SubmitScoreRequest(
    string MapKey,
    Difficulty Difficulty,
    int TimeMs,
    int Value,
    List<WeaponStatDto> WeaponStats
);

public record ScoreDto(
    Guid Id,
    string Player,
    string Map,
    Difficulty Difficulty,
    int Value,
    int TimeMs,
    List<WeaponStatDto> WeaponStats
);

public record LeaderboardRowDto(
    int Rank,
    Guid ScoreId,
    string PlayerDisplayName,
    Guid PlayerId,
    int TimeMs,
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