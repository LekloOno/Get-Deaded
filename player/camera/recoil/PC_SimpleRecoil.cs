using Godot;

public partial class PC_SimpleRecoil(PC_Recoil recoilController) : Node
{
    private PC_Recoil _recoilController = recoilController;
    private Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        SetProcess(false);
    }
    public override void _Process(double delta)
    {
        Vector2 appliedVel = _velocity * (float)delta;
        _recoilController.CameraControl.RotateXClamped(appliedVel.Y);
        _recoilController.CameraControl.RotateFlatDir(appliedVel.X);

        _velocity *= _recoilController.Resistance;
        if (_velocity.Length() < _recoilController.StopThreshold)
        {
            _velocity = Vector2.Zero;
            SetProcess(false);
        }
    }

    public void AddRecoil(Vector2 velocity) => _velocity += velocity;
    public void AddCappedRecoil(Vector2 velocity, float max)
    {
        _velocity += velocity;
        if (_velocity.Length() > max)
            _velocity = _velocity.Normalized() * max;
    }
}