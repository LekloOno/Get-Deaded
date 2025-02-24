// Handles virtual inputs for crouch, slide, and dash.
// Dispatches to slide and dash. Slide then dispatch to crouch if necessary.
// Processes the real input into a Hold/Tap mode input, and checks for Shrinkability.
using Godot;

[GlobalClass]
public partial class PI_CrouchDispatcher : Node
{
    [ExportCategory("User Settings")]
    [Export] public bool Hold = true;

    [ExportCategory("Setup")]
    [Export] private PM_Controller _controller;
    [Export] private PB_Scale _body;
    [Export] private PI_Slide _slideInput;
    [Export] private PI_Dash _dashInput;

    public bool IsCrouched => _active || _tryingUncrouch;
    public ulong LastCrouchDown {get; private set;} = 0; 
    private bool _active = false;
    private bool _tryingUncrouch = false;
    private BoxShape3D _orignalShape;

    public override void _Ready()
    {
        _orignalShape = new BoxShape3D();
        BoxShape3D capsuleShape = (BoxShape3D)_body.Shape;
        _orignalShape.Size = capsuleShape.Size;
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

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("crouch"))
        {
            LastCrouchDown = Time.GetTicksMsec();
            _dashInput.KeyDown();

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

        else if(@event.IsActionReleased("crouch") && Hold)
            TryStop();
    }

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
}