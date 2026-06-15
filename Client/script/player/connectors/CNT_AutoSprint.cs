using System;
using Godot;

/// <summary>
/// Little brief of the behavior of the auto sprint -
/// 
/// The auto sprint tries* to tigger whenever -
/// - The "forward" key is pressed down
/// - The "forward" key is already pressed, and -
///     - any other key is pressed
///     - The ADS is stopped
///     - The slide is stopped
///     - The crouch is stopped
///     - The grace period is over
/// 
/// Besides, the auto sprint automatically stops the sprint on fire.
/// It is not a rule inherent to sprint, but to auto sprint specifically,
/// as it could feel awkward to shoot while sprinting for some players, and is
/// a substantial buff for the player that manage to handle it.
/// 
/// However, there's further rules to allow the player to sprint and shoot, even
/// with auto sprint, explained in GRACE PERIOD.
/// 
/// *By "tries", it means, it will always fire under these conditions
/// but some other conditions might forbid the sprint.
/// For example, if the "forward" key is pressed, the auto sprint fires, but
/// if it the mean time, the player is crouched, the final input won't fire.
/// 
/// GRACE PERIOD
/// 
/// When the player shoots, the auto sprint fires a stop sprint event.
/// At this first shot, the grace period begins.
/// 
/// During this period, no sprint can be triggered anymore
/// 
/// *Unless the player has _gracePressKeyToSprint enabled, in which case, pressing
/// any key while the forward is pressed can retrigger a sprint, that won't be called by
/// further shots during the grace period.
///
/// The grace period is restarted for each new shot during the grace.
/// 
/// At the end of the grace period, the keypressed, ADS, slide & crouch triggers are
/// re-enabled and the sprint is automatically restarted if the "forward" key is pressed.
/// </summary>
[GlobalClass]
public partial class CNT_AutoSprint : Node
{
    [Export] private PI_Sprint _sprintInput;
    [Export] private PI_Walk _walkInput;
    [Export] private PI_Crouch _crouchInput;
    [Export] private PI_Slide _slideInput;
    [Export] private PW_WeaponsHandler _weaponsHandler;

    private bool _enabled;

    public override void _Ready()
    {
        SetPhysicsProcess(false);
        SetProcessUnhandledInput(false);
        if (SprintModeSetting.Mode == SprintMode.Auto)
            Enable();

        SprintModeSetting.ValueChanged += OnModeSettingChanged;
        AutoSprintDelaySetting.ValueChanged += OnDelaySettingChanged;
    }

    private void OnDelaySettingChanged(GodotObject sender, ulong value)
    {
        _graceShotWindow = value;
    }

    private void OnModeSettingChanged(GodotObject? _, SprintMode value)
    {
        if (value == SprintMode.Auto)
            Enable();
        else
            Disable();
    }

    private void Disable()
    {
        if (!_enabled)
            return;

        _enabled = false;
        AddChild(_sprintInput);
        
        SetProcessUnhandledInput(false);
        SetPhysicsProcess(false);

        _weaponsHandler.Shot -= HandleShot;
        _weaponsHandler.Shot -= GraceShot;

        _weaponsHandler.ADSStopped -= CheckSprint;
        _walkInput.KeyPressed -= OnKeyPressed;
        _crouchInput.OnStopInput -= OnCrouchStopped;
        _slideInput.OnStopInput -= OnCrouchStopped;
    }

    private void OnCrouchStopped(object? sender, EventArgs e) =>
        CheckSprint();

    private void Enable()
    {
        if (_enabled)
            return;

        _enabled = true;
        _sprintInput.GetParent().RemoveChild(_sprintInput);
        
        _weaponsHandler.Shot += HandleShot;
        _weaponsHandler.ADSStopped += CheckSprint;
        _walkInput.KeyPressed += OnKeyPressed;
        _crouchInput.OnStopInput += OnCrouchStopped;
        _slideInput.OnStopInput += OnCrouchStopped;
    }

    private void CheckSprint()
    {
        if (_sprintInput.Active)
            return;

        if (!_walkInput.IsForwarding())
            return;

        StartSprint();
    }

    private void OnKeyPressed(object? sender, KeyPressedArgs e) =>
        CheckSprint();

    private ulong _lastGraceShot = 0;
    [Export] private ulong _graceShotWindow = 800;
    [Export] private bool _gracePressKeyToSprint = false;
    // Ensures if _gracePressKeyToSprint is changed during the grace period,
    // The key pressed event isn't double-added
    private bool _gracePressKeyToSprintBuffer = false;
    private void HandleShot()
    {
        StopSprint();
        _weaponsHandler.Shot -= HandleShot;
        StartGrace();
    }

    private void StartGrace()
    {
        GraceShot();
        SetPhysicsProcess(true);
        SetProcessUnhandledInput(true);

        _weaponsHandler.Shot += GraceShot;

        _gracePressKeyToSprintBuffer = _gracePressKeyToSprint;
        if (!_gracePressKeyToSprintBuffer)
            _walkInput.KeyPressed -= OnKeyPressed;
        
        _weaponsHandler.ADSStopped -= CheckSprint;
        _crouchInput.OnStopInput -= OnCrouchStopped;
        _slideInput.OnStopInput -= OnCrouchStopped;
    }

    

    private void ResetGrace()
    {
        SetPhysicsProcess(false);
        SetProcessUnhandledInput(false);
        _weaponsHandler.Shot -= GraceShot;

        if (!_gracePressKeyToSprintBuffer)
            _walkInput.KeyPressed += OnKeyPressed;

        _weaponsHandler.Shot += HandleShot;

        _weaponsHandler.ADSStopped += CheckSprint;
        _crouchInput.OnStopInput += OnCrouchStopped;
        _slideInput.OnStopInput += OnCrouchStopped;
    }

    private void GraceShot() =>
        _lastGraceShot = PHX_Time.ScaledTicksMsec;

    public override void _PhysicsProcess(double delta)
    {
        if ((PHX_Time.ScaledTicksMsec - _lastGraceShot) >= _graceShotWindow)
        {
            ResetGrace();
            CheckSprint();
        }
    }

    private void StartSprint() =>
        _sprintInput.HandleExternal(PI_ActionState.STARTED, new());
    
    private void StopSprint() =>
        _sprintInput.HandleExternal(PI_ActionState.STOPPED, new());

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("auto_sprint_quick_start"))
        {
            ResetGrace();
            CheckSprint();
        }
    }
}