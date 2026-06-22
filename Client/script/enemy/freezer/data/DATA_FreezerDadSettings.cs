using System;
using Godot;

[GlobalClass]
public partial class DATA_FreezerDadSettings : Resource
{
    [Export] public uint        Score   { get; private set; } = 50;
    [Export] public float ChildSpawnMinRadius { get; set; } = 1.5f;
    [Export] public float ChildSpawnMaxRadius { get; set; } = 4.0f;

    public void UpdateFrom(E_EnemySettings settings)
    {
        Score = settings.Score;

        Updated?.Invoke();
    }

    public event Action? Updated;
}