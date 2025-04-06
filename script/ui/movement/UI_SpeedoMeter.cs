using Godot;

public partial class UI_SpeedoMeter : Label3D
{
    [ExportCategory("Settings")]
    [Export] private Gradient _gradient;
    [Export] private float _minSpeed = 60f;
    [Export] private float _maxSpeed = 10f;

    [ExportCategory("Setup")]
    [Export] private PM_Controller _controller;

    public override void _Process(double delta)
    {
        Vector3 faltVel = MATH_Vector3Ext.Flat(_controller.RealVelocity);
        float speed = faltVel.Length() * 3.6f;
        float scaledSpeed = (speed - _minSpeed)/(_maxSpeed-_minSpeed);
        scaledSpeed = Mathf.Clamp(scaledSpeed, 0, 1);

        SetText((int)speed + " km/h");
        SetModulate(_gradient.Sample(scaledSpeed));
    }
}
