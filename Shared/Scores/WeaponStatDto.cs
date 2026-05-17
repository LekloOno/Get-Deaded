namespace Shared.Scores;

public record WeaponStatDto(
    string WeaponKey,
    int Damage,
    int Kills,
    float Accuracy,
    float CriticalAccuracy
);