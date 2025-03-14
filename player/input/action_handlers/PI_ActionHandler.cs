using Godot;

public delegate void ActionInputEvent<T>(object sender, T args);

public abstract partial class PI_ActionHandler<T> : Node, PI_InputAction
{
    public ActionInputEvent<T> Start;
    public ActionInputEvent<T> Stop;
    public ActionInputEvent<T> Perform;

    // Defines how the received input should be processed.
    // The InputEvent will either come from _UnhandledInput or _UnhandledKeyInput depending on the concrete implementation.
    protected abstract void HandleInput(InputEvent @event);

    // Used to handle a *fake* external input.
    //  -   Typically, multiple individuals Input Handlers could interact with each others.
    //      One input start could mean the cancellation of another.
    protected abstract void HandleExternal(PI_ActionState actionState, T value);
    
    // Extract an input value from a given InputEvent
    //  -   The implementing class could use some of its own internal state, or anything else.
    //      e.g. the strength of a jump depending on how the input was performed.
    protected abstract T GetInputValue(InputEvent @event);
    public abstract void EnableAction();
    public abstract void DisableAction();

    protected void Send(PI_ActionState actionState, T value)
    {
        switch (actionState)
        {
            case PI_ActionState.STARTED:
                Start?.Invoke(this, value);
                break;
            case PI_ActionState.STOPPED:
                Stop?.Invoke(this, value);
                break;
            case PI_ActionState.PERFORMED:
                Perform?.Invoke(this, value);
                break;
            default:
                break;
        }
    }
}