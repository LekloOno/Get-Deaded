using Godot;

[GlobalClass]
public partial class WE_Death : Node
{
    [Export] private PM_Controller _playerController;
    [Export] private float _holdTime = 1.5f;
    [Export] private float _releaseTime = .8f;
    private float _chargeTracker = 0f;
    private float _chargeTime = 0f;
    private bool _reviveDown;

    public override void _Ready()
    {
        SetProcessUnhandledInput(false);
        SetPhysicsProcess(false);
        _playerController.OnDie += (o, e) => SetProcessUnhandledInput(true);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("revive"))
        {
            SetPhysicsProcess(true);
            _chargeTime = _holdTime;
        }
        else if (@event.IsActionReleased("revive"))
            _chargeTime = -_releaseTime;
    }

    public override void _PhysicsProcess(double delta)
    {
        _chargeTracker += (float)delta/_chargeTime;
        if (_chargeTracker >= 1f)
        {
            SetProcessUnhandledInput(false);
            _playerController.Revive();
            _chargeTracker = 0f;
        }
        else if (_chargeTracker > 0f)
            return;

        SetPhysicsProcess(false);
    }
}