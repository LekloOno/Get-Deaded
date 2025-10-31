using Godot;


/// <summary>
/// Handles virtual inputs for crouch and slide.
/// Dispatches to slide. Slide then dispatches to crouch if necessary.
/// Processes the real input into a Hold/Tap mode input, and checks for Shrinkability.
/// 
/// The input value holds the crouch strength, which could be used for analog scaling.
/// </summary>
[GlobalClass]
public partial class PI_CrouchDispatcher : PI_ActionHandler<float>
{
    [ExportCategory("User Settings")]
    [Export] public bool Hold = true;

    [ExportCategory("Setup")]
    [Export] private PM_Controller _controller;
    [Export] private PHX_BodyScale _body;
    [Export] private PI_Slide _slideInput;

    public bool IsCrouched => _active || _tryingUncrouch;
    public ulong LastCrouchDown {get; private set;} = 0; 
    private bool _active = false;
    private bool _tryingUncrouch = false;

    protected override void HandlerReady()
    {
        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        if(PHX_Checks.CanUncrouch(_controller, _body))
        {
            _slideInput.InputStop();
            _tryingUncrouch = false;
            SetPhysicsProcess(false);
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event) => HandleInput(@event);

    private void TryStop()
    {
        _active = false;
        if(PHX_Checks.CanUncrouch(_controller, _body))
        {
            _slideInput.InputStop();
        }
        else
        {
            _tryingUncrouch = true;
            SetPhysicsProcess(true);
        }
    }

    private void TryStart()
    {
        LastCrouchDown = Time.GetTicksMsec();
        if (_active && !Hold)
            TryStop();
        else 
        {
            _active = true;
            if (_tryingUncrouch)
            {
                _tryingUncrouch = false;
                SetPhysicsProcess(false);
            }
            else
                _slideInput.InputStart();
        }
    }

    protected override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("crouch"))
            TryStart();

        else if(@event.IsActionReleased("crouch") && Hold)
            TryStop();
    }


    public override void HandleExternal(PI_ActionState actionState, float value)
    {
        switch (actionState)
        {
            case PI_ActionState.STARTED :
                TryStart();
                break;
            case PI_ActionState.STOPPED :
                TryStop();
                break;
            default :
                break;
        }
    }

    protected override float GetInputValue(InputEvent @event) => 1f;

    public override void EnableAction() => SetProcessUnhandledKeyInput(true);

    public override void DisableAction()
    {
        TryStop();
        SetProcessUnhandledKeyInput(false);
    }

}