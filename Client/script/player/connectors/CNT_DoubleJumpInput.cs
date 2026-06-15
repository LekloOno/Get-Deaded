using System;
using Godot;

[GlobalClass]
public partial class CNT_DoubleJumpInput : Node
{
    [Export] private CNT_InternalDashInput _dashInput  = null!;
    [Export] private PI_Jump               _jumpInput  = null!;
    [Export] private PS_Grounded           _groundState = null!;
    [Export] private float _minHeight       = 0.28f;
    [Export] private ulong _dashJumpWindow  = 100;

    public event Action? Started;

    private DoubleJumpMode _mode = 0;

    public override void _Ready()
    {
        SetMode(this, DoubleJumpModeSetting.Mode);
        DoubleJumpModeSetting.ValueChanged += SetMode;
    }

    private void SetMode(GodotObject _, DoubleJumpMode mode)
    {
        SetProcessUnhandledInput(mode.HasFlag(DoubleJumpMode.Defined));
        SetDashJump(mode);

        _mode = mode;
    }

    private void SetDashJump(DoubleJumpMode mode)
    {
        bool prevDash = _mode.HasFlag(DoubleJumpMode.DashJump);
        bool nextDash =  mode.HasFlag(DoubleJumpMode.DashJump);

        if (prevDash == nextDash)
            return;

        if (nextDash)
            _dashInput.Dashed += OnInternalDashStart;
        else
            _dashInput.Dashed -= OnInternalDashStart;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("double_jump"))
            Started?.Invoke();
    }

    private bool OnInternalDashStart()
    {
        if (PHX_Time.ScaledTicksMsec - _jumpInput.LastInput > _dashJumpWindow)
            return false;
            
        Started?.Invoke();
        return true;
    }

    public void TryHeightDoubleJump()
    {
        if (!_mode.HasFlag(DoubleJumpMode.HeightJump))
            return;
        
        if (_groundState.DistanceToGround < _minHeight)
            return;
            
        if (!_jumpInput.UseBuffer())
            return;

        Started?.Invoke();
    }
}