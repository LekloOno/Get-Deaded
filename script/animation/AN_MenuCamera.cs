using Godot;

public partial class AN_MenuCamera : Node3D
{
    [Export] private float _degreePerSec;

    public override void _Process(double delta)
    {
        RotateY(Mathf.DegToRad(_degreePerSec * (float) delta));
    }
}