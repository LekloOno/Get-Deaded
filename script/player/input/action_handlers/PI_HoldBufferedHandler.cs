using Godot;

/// <summary>
/// Just like BufferedHandler, but empties the buffer whenever the key is released.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class PI_HoldBufferdHandler<T> : PI_BufferedHandler<T>
{
    protected override void InputUp(InputEvent @event)
    {
        T value = GetInputValue(@event);
        Stop?.Invoke(this, value);
        Empty();
    }
}