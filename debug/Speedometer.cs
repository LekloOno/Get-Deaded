using Godot;

public partial class Speedometer : Label
{
    [Export] public PM_Controller Controller;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Vector3 faltVel = Controller.RealVelocity * new Vector3(1, 0, 1);
        SetText((int)(faltVel.Length() * 3.6) + "");
    }
}
