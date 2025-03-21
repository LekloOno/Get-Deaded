using Godot;

[GlobalClass]
public partial class PC_Recoil : Node3D
{
    [Export] private Vector2 _resistance;
    [Export] private float _stopThreshold;
    [Export] private PC_Control _cameraControl;
    private Vector2 _velocity;
    public override void _Ready()
    {
        SetProcess(false);
    }
    public override void _Process(double delta)
    {
        Vector2 appliedVel = _velocity * (float)delta;
        _cameraControl.RotateXClamped(appliedVel.Y);
        _cameraControl.RotateFlatDir(appliedVel.X);

        _velocity *= _resistance;
        if (_velocity.Length() < _stopThreshold)
        {
            _velocity = Vector2.Zero;
            SetProcess(false);
        }
    }

    public void AddRecoil(Vector2 velocity)
    {
        _velocity += velocity;
        SetProcess(true);
    }

    public void AddCappedRecoil(Vector2 velocity, float max)
    {
        _velocity += velocity;
        if (_velocity.Length() > max)
            _velocity = _velocity.Normalized() * max;
        SetProcess(true);
    }
}