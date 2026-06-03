using Godot;

public partial class E_EnemyScreenOptimizer : Node3D
{
    [Export] private AnimationTree? _animationTree;

    public override void _PhysicsProcess(double delta)
    {
        if ((Engine.GetPhysicsFrames() & 7) != 7)
            return;

        if (GetViewport().GetCamera3D() is not Camera3D camera)
            return;

        Vector3 toEnemy = (GlobalPosition - camera.GlobalPosition).Normalized();
        Vector3 forward = -camera.GlobalTransform.Basis.Z;

        float dot = forward.Dot(toEnemy);

        if (dot > 0.2f)
            OnScreenEntered();
        else
            OnScreenExited();
    }

    private void OnScreenExited()
    {
        _animationTree.Active = false;
    }

    private void OnScreenEntered()
    {
        _animationTree.Active = true;
    }
}