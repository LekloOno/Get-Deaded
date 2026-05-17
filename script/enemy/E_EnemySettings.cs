using System;
using Godot;

[GlobalClass]
public partial class E_EnemySettings : Resource
{
    [Export] public PackedScene Fire {get; private set;}
    [Export] public GCD_Health Health {get; private set;}
    [Export] public PROTO_MoverData MoverData {get; private set;}
    [Export] public uint Score {get; private set;} = 50;
    [Export] public float SpreadFactor {get; private set;} = 15f;
    [Export] public double ReactionTime {get; private set;} = 0.2f;

    public void UpdateFrom(E_EnemySettings settings)
    {
        Fire = settings.Fire;
        Health = settings.Health;
        MoverData = settings.MoverData;
        Score = settings.Score;
        SpreadFactor = settings.SpreadFactor;
        ReactionTime = settings.ReactionTime;

        Updated?.Invoke();
    }

    public Action Updated;
}