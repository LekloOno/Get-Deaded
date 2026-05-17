namespace Shared.Scores;

public record WeaponStatDto(
    string WeaponKey,
    int DamageDealt,
    int Kills,
    float Accuracy,
    float CriticalAccuracy
);