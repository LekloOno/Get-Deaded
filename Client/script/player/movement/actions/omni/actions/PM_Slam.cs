using Godot;

[GlobalClass]
public partial class PM_Slam : PM_Action
{
    [Export] private PM_Controller  _controller = null!;
    [Export] private CNT_SlamInput  _input = null!;
    [Export] private PM_OmniCharge  _charge = null!;
    [Export] private DATA_Slam      _data = null!;
    [Export] private PM_LedgeClimb  _ledgeClimb    = null!;

    [Export] private ulong _superGlideGraceWindow = 80;

    private bool _isSlamming;

    public override void _Ready()
    {
        SetPhysicsProcess(false);
        _input.Started += TryPerformAction;
    }

    private void TryPerformAction()
    {
        if (_isSlamming)
            return;

        if (_ledgeClimb.IsClimbing)
            return;

        if (PHX_Time.ScaledTicksMsec - _ledgeClimb.LastSuperGlide < _superGlideGraceWindow)
            return;

        if (!_charge.TryConsume(_data.ChargeCost))
            return;

        StartSlam();
    }


    private Vector3 _prevRealVelocity;
    private Vector3 _cachedForce;
    public void StartSlam()
    {
        _prevRealVelocity = _controller.RealVelocity;
        Vector3 velocity = _prevRealVelocity;

        float appliedSpeed = Mathf.Max(_data.Speed, velocity.Length());
        
        _cachedForce = appliedSpeed * Vector3.Down;

        _controller.TakeOverForces.AddPersistent(_cachedForce);
        _isSlamming = true;

        StartDuration();
        InvokeStart();
    }

    private double _acc = 0;
    public override void _PhysicsProcess(double delta)
    {
        if (_acc >= _data.Duration)
            AbortSlam();

        _acc += delta;
    }

    private void StartDuration()
    {
        _acc = 0;
        SetPhysicsProcess(true);
    }

    public void AbortSlam()
    {
        _controller.TakeOverForces.RemovePersistent(_cachedForce);

        _isSlamming = false;
        SetPhysicsProcess(false);

        InvokeStop();
    }

    private void EndDash()
    {
        Vector3 outVelocity = OutVelocity();

        _controller.Velocity = outVelocity;
        _controller.RealVelocity = outVelocity;

        AbortSlam();
    }

    private Vector3 OutVelocity()
    {
        return _prevRealVelocity.Length() * Vector3.Down;
    }
}