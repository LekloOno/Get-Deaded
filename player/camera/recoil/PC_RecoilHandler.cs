using Godot;

public abstract partial class PC_RecoilHandler(PC_Recoil recoilController) : Node
{
    protected PC_Recoil _recoilController = recoilController;
    protected Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        SetProcess(false);
    }
    public override void _Process(double delta)
    {
        ApplyVelocity(delta);
        ApplyResistance(delta);
        CheckThreshold();

        CustomProcess();
    }

    public void AddRecoil(Vector2 velocity)
    {
        _velocity += velocity;
        CustomAdd(velocity);
    }
    public void AddCappedRecoil(Vector2 velocity, float max)
    {
        _velocity += velocity;
        if (_velocity.Length() > max)
            _velocity = _velocity.Normalized() * max;
            
        CustomAddCapped(velocity, max);
    }

    protected void ApplyResistance(double delta) => _velocity -= _velocity * _recoilController.Resistance * (float)delta;
    protected void ApplyVelocity(double delta)
    {
        Vector2 appliedVel = _velocity * (float)delta;
        _recoilController.CameraControl.RotateXClamped(appliedVel.Y);
        _recoilController.CameraControl.RotateFlatDir(appliedVel.X);
    }
    protected void CheckThreshold()
    {
        if (_velocity.Length() > _recoilController.StopThreshold)
            return;
            
        _velocity = Vector2.Zero;
        OnThresholdReached();
    }

    protected abstract void OnThresholdReached();
    protected abstract void CustomProcess();
    protected abstract void CustomAdd(Vector2 velocity);
    protected abstract void CustomAddCapped(Vector2 velocity, float max);
}