using Godot;

public partial class PC_ResetRecoil(PC_Recoil recoilController, float resetSpeed) : PC_RecoilHandler(recoilController)
{
    private Vector2 _initialRotation = Vector2.Zero;
    private Vector2 _initialVelocity = Vector2.Zero;
    private Vector2 _resetDir = Vector2.Zero;
    private float _resetSpeed = resetSpeed;
    private bool _ascending = false;

    public override void _Process(double delta)
    {
        if (_ascending)
            base._Process(delta);
        else
        {
            Vector2 currentRot = _recoilController.CameraControl.CurrentRotation();
            if (currentRot.X <= _initialRotation.X)
            {
                _velocity = Vector2.Zero;
                SetProcess(false);
                return;
            }

            _velocity -= _resetDir * (float)delta;
            ApplyVelocity(delta);
        }
    }

    protected override void OnThresholdReached() => _ascending = false;
    protected override void CustomProcess(){}

    protected override void CustomAdd(Vector2 velocity)
    {
        _ascending = true;
        _initialRotation = _recoilController.CameraControl.CurrentRotation();
        _initialVelocity = velocity;
        _resetDir = velocity.Normalized() * _resetSpeed;
    }

    protected override void CustomAddCapped(Vector2 velocity, float max)
    {
        _ascending = true;
        _initialRotation = _recoilController.CameraControl.CurrentRotation();
        _initialVelocity = velocity;
        _resetDir = velocity.Normalized() * _resetSpeed;
    }
}