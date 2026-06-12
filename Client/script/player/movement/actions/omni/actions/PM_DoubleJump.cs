using Godot;

[GlobalClass]
public partial class PM_DoubleJump : PM_Action
{
    [Export] private PM_Controller          _controller = null!;
    [Export] private CNT_DoubleJumpInput    _input = null!;
    [Export] private PM_OmniCharge          _charge = null!;
    [Export] private DATA_DoubleJump        _data = null!;

    public override void _Ready()
    {
        SetPhysicsProcess(false);
        _input.Started += TryPerformAction;
    }

    private double _acc = 0;
    public override void _PhysicsProcess(double delta)
    {
        if (_acc >= _data.Duration)
            AbortDash();

        _acc += delta;
    }

    private bool    _isPropelling;
    private Vector3 _cachedForce;
    private Vector3 Force => _data.Strength * Vector3.Up;

    private void TryPerformAction()
    {
        if (_charge.TryConsume(_data.ChargeCost))
            PerformAction();
    }

    private void PerformAction()
    {
        if (_isPropelling)
            AbortDash();

        Vector3 velocity = _controller.RealVelocity;

        float horPenalty = 1 - _data.HorizontalPenalty;
        float vertPenalty = 1 - _data.VerticalPenalty;
        
        velocity *= new Vector3(horPenalty, vertPenalty, horPenalty);

        _controller.RealVelocity = velocity;
        _controller.Velocity = velocity;

        _cachedForce = Force;
        _controller.AdditionalForces.AddPersistent(_cachedForce);
        _isPropelling = true;

        StartDuration();
        InvokeStart();
    }

    private void StartDuration()
    {
        _acc = 0;
        SetPhysicsProcess(true);
    }

    public void AbortDash()
    {
        _controller.AdditionalForces.RemovePersistent(_cachedForce);

        _isPropelling = false;
        SetPhysicsProcess(false);

        InvokeStop();
    }
}