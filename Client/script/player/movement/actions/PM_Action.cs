using System;
using Godot;

public abstract partial class PM_Action : Node
{
    public event Action? Started;
    public event Action? Stopped;

    protected void InvokeStart() =>
        Started?.Invoke();

    protected void InvokeStop() =>
        Stopped?.Invoke();
}