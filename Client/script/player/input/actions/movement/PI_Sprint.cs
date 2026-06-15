using System;
using Godot;

[GlobalClass]
public partial class PI_Sprint : PI_HoldableHandler<EmptyInput>
{

    [ExportCategory("Setup")]
    [Export] private PM_Jump _jump;
    [Export] private PI_Walk _walkInput;

    public EventHandler OnStartSprinting;
    public EventHandler OnStopSprinting;

    protected override ACTIONS_Action Action => ACTIONS_Movement.SPRINT;
    protected override EmptyInput GetInputValue(InputEvent @event) => EmptyInput.NONE;

    protected override void HandlerReady()
    {
        //_jump.OnStart += HandleExternalStop;
        _walkInput.OnStopOrBackward += (o, f) => HandleExternalStop();
        SprintModeSetting.ValueChanged += OnModeSettingChanged;
    }

    private void OnModeSettingChanged(GodotObject? _, SprintMode value)
    {
        switch (value)
        {
            case SprintMode.Auto :
                return;
            
            case SprintMode.Hold :
                Hold = true;
                break;
            
            case SprintMode.Toggle :
                Hold = false;
                break;
        }
    }


    private void HandleExternalStop() =>
        HandleExternal(PI_ActionState.STOPPED, new());

    public override void _UnhandledInput(InputEvent @event)
    {
        HandleInput(@event);
    }


    public override void EnableAction() => SetProcessUnhandledInput(true);
    public override void DisableAction()
    {
        SetProcessUnhandledInput(false);
        HandleExternal(PI_ActionState.STOPPED, new());
    }
}