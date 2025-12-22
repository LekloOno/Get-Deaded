using Godot;

namespace Pew;

public struct EmptyInput { 
    public static EmptyInput NONE = new();
}

public delegate void ActionInputEvent<T>(object sender, T args);

public abstract partial class PI_ActionHandler<T> : Node, PI_InputAction
{
    public ActionInputEvent<T> Start;
    public ActionInputEvent<T> Stop;
    public ActionInputEvent<T> Perform;

    /// <summary>
    /// Action Input Handler disables the input handling by default.
    /// The right handling should be enabled through InputAction.EnableAction().
    /// For further initialization operations, see PI_ActionHandler.HandlerReady().
    /// </summary>
    public sealed override void _Ready()
    {
        SetProcessUnhandledInput(false);
        SetProcessUnhandledKeyInput(false);
        HandlerReady();
    }

    /// <summary>
    /// For consistency reasons, Action Handler should override HandlerReady instead of _Ready when implemnting new initialization operations.
    /// </summary>
    protected virtual void HandlerReady() {}

    /// <summary>
    /// Defines how the received input should be processed
    /// The InputEvent will either come from _UnhandledInput or _UnhandledKeyInput depending on the concrete implementation.
    /// </summary>
    /// <param name="event">The received input, from _UnhandledInput or _UnhandledKeyInput.</param>
    protected abstract void HandleInput(InputEvent @event);

    /// <summary>
    /// Used to handle a "fake" external input.
    /// -   Typically, multiple individuals Input Handlers could interact with each others.
    ///     One input start could mean the cancellation of another.
    /// </summary>
    /// <param name="actionState">The requested action state.</param>
    /// <param name="value">The virtual input value.</param>
    public abstract void HandleExternal(PI_ActionState actionState, T value);
    
    /// <summary>
    /// Extract an input value from a given InputEvent
    /// -   The implementing class could use some of its own internal state, or anything else.
    ///     e.g. the strength of a jump depending on how the input was performed.
    /// </summary>
    /// <param name="event">The received input.</param>
    /// <returns></returns>
    protected abstract T GetInputValue(InputEvent @event);

    public abstract void EnableAction();
    public abstract void DisableAction();

    /// <summary>
    /// A simple default input sender.
    /// Call the event corresponding to the given Action State.
    /// </summary>
    /// <param name="actionState">The Action State event to send.</param>
    /// <param name="value">The value of the input.</param>
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