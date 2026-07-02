namespace Shared.Scores;

public record SubmitScoreRequest<T>(
    string MapKey,
    int Difficulty,
    int TimeMs,
    int Score,
    List<WeaponStatDto> WeaponStats,
    T ModeDetails
);

public record ScoreDto<T>(
    Guid Id,
    string Player,
    string Map,
    string ModeKey,
    string VersionKey,
    int Difficulty,
    int TotalScore,
    int TimeSpentMs,
    List<WeaponStatDto> WeaponStats,
    T ModeDetails
);

public record LeaderboardRowDto<T>(
    int Rank,
    Guid ScoreId,
    string Player,
    Guid PlayerId,
    int TimeMs,
    int Score,
    int Kills,
    float Damage,
    string BestWeaponKey,
    float? BestWeaponAccuracy,
    T ModeDetails
);

public record LeaderboardPageDto<T>(
    int StartRank,
    int EndRank,
    List<LeaderboardRowDto<T>> Entries
);

public record SubmitScoreResponse(
    Guid ScoreId,
    int Rank
);