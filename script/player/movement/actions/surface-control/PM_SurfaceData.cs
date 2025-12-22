using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PM_SurfaceData : Resource
{
    [Export] public float MaxSpeed {get; private set;}
    [Export] public float MaxAccel {get; private set;}
    [Export] public float Drag {get; private set;}

    public EventHandler OnStart;
    public EventHandler OnStop;
}