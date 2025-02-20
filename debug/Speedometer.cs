using Godot;

public partial class Speedometer : Label
{
    [Export] private PM_Controller _controller;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Vector3 faltVel = _controller.RealVelocity * new Vector3(1, 0, 1);
        SetText((int)(faltVel.Length() * 3.6) + "");
    }
}
