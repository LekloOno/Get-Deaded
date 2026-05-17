using System;
using Godot;

[GlobalClass]
public partial class PM_Lurch : PM_Action
{
    [Export] private PI_Walk _walkInput;
    [Export] private PM_Controller _controller;
    [Export] private PM_LedgeClimb _ledgeClimb;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_OmniCharge _charge;
    [Export] private float _chargeCost = 25f;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("lurch"))
            StartLurch();
    }

    public void StartLurch()
    {
        if (_groundState.IsGrounded())
            return;

        if (_ledgeClimb.IsClimbing)
            return;
        
        if (!_charge.TryConsume(_chargeCost))
            return;

        Vector3 direction = GetLurchDirection();
        float prevSpeed = _controller.RealVelocity.Length();
        Vector3 nextVelocity = direction * prevSpeed;

        _controller.Velocity = nextVelocity;
        _controller.RealVelocity = nextVelocity;

        InvokeStart();
    }

    public Vector3 GetLurchDirection()
    {
        if (_walkInput.WalkAxis != Vector2.Zero)
            return _walkInput.WishDir;
        
        return _walkInput.FlatDir;
    }
}