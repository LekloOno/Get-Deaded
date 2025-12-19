using Godot;

[GlobalClass]
public partial class PM_Mover : GM_Mover
{
    [Export] private PI_Walk _walkProcess;
    [Export] private PM_Controller _controller;
    public override Vector3 Velocity => _controller.RealVelocity;

    public override Vector3 WishDir => _walkProcess.WishDir;

    public override Vector2 WalkAxis => _walkProcess.WalkAxis;
}