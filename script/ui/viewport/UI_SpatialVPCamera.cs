using Godot;

namespace Pew;

public partial class UI_SpatialVPCamera : Camera3D
{
    [Export] private Node3D _camera;

    public override void _Process(double delta)
    {
        GlobalTransform = _camera.GlobalTransform;
    }
}
