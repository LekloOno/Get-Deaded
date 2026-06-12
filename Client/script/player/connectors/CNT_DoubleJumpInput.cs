using System;
using Godot;

public partial class CNT_DoubleJumpInput : Node
{
    [Export] private PI_Jump        _jumpInput = null!;
    [Export] private PI_Dash        _dashInput = null!;
    [Export] private PS_Grounded    _groundState = null!;
    [Export] private float _minHeight       = 0.35f;
    [Export] private ulong _dashJumpWindow  = 100; 

    private readonly PI_Dash _internalDashInput = new();

    public event Action? Started;

    private DoubleJumpMode _mode = 0;

    public override void _Ready()
    {
        _dashInput.GetParent().RemoveChild(_dashInput);
        AddChild(_internalDashInput);
        _internalDashInput.OnStartInput += OnInternalDashStart;

        SetMode(DoubleJumpModeSetting.Mode);
        DoubleJumpModeSetting.ModeChanged += SetMode;
    }

    private void SetMode(DoubleJumpMode mode)
    {
        SetProcessUnhandledInput(mode.HasFlag(DoubleJumpMode.Defined));
        SetDashJump(mode);
        SetHeightJump(mode);
    }

    private void SetHeightJump(DoubleJumpMode mode)
    {
        bool prevHeight = _mode.HasFlag(DoubleJumpMode.HeightJump);
        bool nextHeight =  mode.HasFlag(DoubleJumpMode.HeightJump);

        if (prevHeight == nextHeight)
            return;

        if (nextHeight)
            _jumpInput.Start += TryHeightDoubleJump;
        else
            _jumpInput.Start -= TryHeightDoubleJump;
    }

    private void SetDashJump(DoubleJumpMode mode)
    {
        bool prevDash = _mode.HasFlag(DoubleJumpMode.DashJump);
        bool nextDash =  mode.HasFlag(DoubleJumpMode.DashJump);

        if (prevDash == nextDash)
            return;

        if (nextDash)
        {
            RemoveChild(_internalDashInput);
            AddChild(_dashInput);
        }
        else
        {
            AddChild(_internalDashInput);
            RemoveChild(_dashInput);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("double_jump"))
            Started?.Invoke();
    }

    private void OnInternalDashStart(object? sender, EventArgs e)
    {
        if (PHX_Time.ScaledTicksMsec - _jumpInput.LastInput < _dashJumpWindow)
            _dashInput.KeyDown();
        else
            Started?.Invoke();
    }

    private void TryHeightDoubleJump(object sender, float args)
    {
        if (_groundState.DistanceToGround > _minHeight)
            Started?.Invoke();
    }
}