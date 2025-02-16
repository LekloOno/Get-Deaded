using Godot;

[GlobalClass]
public partial class PM_SurfaceStateData : Resource
{
    [Export] public PM_SurfaceData Normal {get; private set;}
    [Export] public PM_SurfaceData Crouch {get; private set;}
    [Export] public PM_SurfaceData Sprint {get; private set;}
    [Export] public PM_SurfaceData Slide {get; private set;}
}