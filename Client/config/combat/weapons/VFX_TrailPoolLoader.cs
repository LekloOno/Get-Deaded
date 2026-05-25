using System.Collections.Generic;
using Godot;

public partial class VFX_TrailPoolLoader : Node
{
    public static VFX_TrailPoolLoader? Instance {get; private set;}
    private static readonly List<VFX_ITrail> _trails = [];

    public static void Register(VFX_ITrail trail)
    {
        _trails.Add(trail);
    }

    public static void Unregister(VFX_ITrail trail)
    {
        _trails.Remove(trail);
    }

    public override void _Ready()
    {
        Instance = this;
        foreach (VFX_ITrail trail in _trails)
            trail.Preload(this);
    }
}