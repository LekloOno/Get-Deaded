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
    [Export] private PB_Scale _body;
    [Export] private PI_Slide _slideInput;
    [Export] private PI_Dash _dashInput;

    public ulong LastCrouchDown {get; private set;} = 0; 
    private bool _active = false;
    private CapsuleShape3D _originalScale;

    public override void _Ready()
    {
        _originalScale = new CapsuleShape3D();
        CapsuleShape3D capsuleShape = (CapsuleShape3D)_body.Shape;
        _originalScale.Height = capsuleShape.Height;
        _originalScale.Radius = capsuleShape.Radius;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("crouch"))
        {
            LastCrouchDown = Time.GetTicksMsec();
            _dashInput.KeyDown();

            if (_active && !Hold)
            {
                _active = false;
                _slideInput.InputStop();
            } else 
            {
                _active = true;
                _slideInput.InputStart();
            }
        }

        else if(@event.IsActionReleased("crouch"))
        {
            if(Hold)
            {
                _active = false;
                _slideInput.InputStop();
            }
        }
    }
}