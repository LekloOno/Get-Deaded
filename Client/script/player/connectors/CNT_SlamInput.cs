using System;
using Godot;

[GlobalClass]
public partial class CNT_SlamInput : Node
{
    [Export] private CNT_InternalDashInput  _dashInput = null!;
    [Export] private PI_CrouchDispatcher    _crouchDispatcher = null!;
    [Export] private PS_Grounded            _groundState = null!;
    [Export] private ulong _dashCrouchWindow  = 100;
    [Export] private ulong _quickCrouchWindow = 50;

    public event Action? Started;

    private SlamMode _mode = 0;
    private ulong _lastCrouchStart = 0;

    public override void _Ready()
    {
        SetMode(this, SlamModeSetting.Mode);
        SlamModeSetting.ValueChanged += SetMode;

        _crouchDispatcher.Start += RegisterStart;
    }

    private void RegisterStart(object sender, float args) =>
        _lastCrouchStart = PHX_Time.ScaledTicksMsec;

    private void SetMode(GodotObject? _, SlamMode mode)
    {
        SetProcessUnhandledInput(mode.HasFlag(SlamMode.Defined));
        SetDashCrouch(mode);
        SetQuickCrouch(mode);

        _mode = mode;
    }

    private void SetQuickCrouch(SlamMode mode)
    {
        bool prevQuick = _mode.HasFlag(SlamMode.QuickCrouch);
        bool nextQuick =  mode.HasFlag(SlamMode.QuickCrouch);

        if (prevQuick == nextQuick)
            return;

        if (nextQuick)
            _crouchDispatcher.Stop += TryQuickCrouchSlam;
        else
            _crouchDispatcher.Stop -= TryQuickCrouchSlam;
    }

    private void SetDashCrouch(SlamMode mode)
    {
        bool prevDash = _mode.HasFlag(SlamMode.DashCrouch);
        bool nextDash =  mode.HasFlag(SlamMode.DashCrouch);

        if (prevDash == nextDash)
            return;

        if (nextDash)
            _dashInput.Dashed += OnInternalDashStart;
        else
            _dashInput.Dashed -= OnInternalDashStart;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("slam"))
            Started?.Invoke();
    }

    private bool OnInternalDashStart()
    {
        if (PHX_Time.ScaledTicksMsec - _crouchDispatcher.LastCrouchDown > _dashCrouchWindow)
            return false;
        
        Started?.Invoke();
        return true;
    }

    private void TryQuickCrouchSlam(object sender, float args)
    {
        if (_groundState.IsGrounded())
            return;

        if (!IsQuickCrouch())
            return;

        Started?.Invoke();
    }

    private bool IsQuickCrouch() => PHX_Time.ScaledTicksMsec - _lastCrouchStart < _quickCrouchWindow; 
}