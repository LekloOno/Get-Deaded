using Godot;

public delegate void ActionInputEvent<T>(object sender, T args);

public abstract partial class PI_ActionHandler<T> : Node
{
    protected abstract ACTIONS_Action Action {get;}

    public ActionInputEvent<T> Start;
    public ActionInputEvent<T> Stop;
    public ActionInputEvent<T> Perform;

    public abstract void HandleInput(InputEvent @event);

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