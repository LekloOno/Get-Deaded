using System;
using Godot;

[GlobalClass]
public partial class E_FreezerSettings : Resource
{
    [Export] public uint        Score   { get; private set; } = 50;
    [Export] public GCD_Health  Health  { get; private set; } = null!;

    public void UpdateFrom(E_EnemySettings settings)
    {
        Health = settings.Health;
        Score = settings.Score;

        Updated?.Invoke();
    }

    public event Action? Updated;
}