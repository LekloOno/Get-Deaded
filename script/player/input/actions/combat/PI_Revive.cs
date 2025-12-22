using Godot;

namespace Pew;

[GlobalClass]
public partial class PI_Revive : PI_InputKeyAction
{
    [Export] private float _holdTime = 1.5f;
    [Export] private float _releaseTime = .8f;
    private float _chargeTracker = 0f;
    private float _chargeTime = 0f;
    private bool _reviveDown;

    public ActionInputEvent<float> Revive;

    public override void _Ready() => SetPhysicsProcess(false);

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed(ACTIONS_Combat.REVIVE))
        {
            SetPhysicsProcess(true);
            _chargeTime = _holdTime;
        }
        else if (@event.IsActionReleased(ACTIONS_Combat.REVIVE))
            _chargeTime = -_releaseTime;
    }

    public override void _PhysicsProcess(double delta)
    {
        _chargeTracker += (float)delta/_chargeTime;
        if (_chargeTracker >= 1f)
        {
            Revive?.Invoke(this, 1f);
            _chargeTracker = 0f;
        }
        else if (_chargeTracker > 0f)
            return;

        SetPhysicsProcess(false);
    }
}