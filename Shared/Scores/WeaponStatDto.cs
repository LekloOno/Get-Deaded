namespace Shared.Scores;

public record WeaponStatDto(
    string WeaponKey,
    float Damage,
    int Kills,
    float Accuracy,
    float CriticalAccuracy
);