using System;
using Godot;

public abstract partial class PM_Action : Node
{
    public event Action OnStart;
    public event Action OnStop;

    protected void InvokeStart() =>
        OnStart?.Invoke();

    protected void InvokeStop() =>
        OnStop?.Invoke();
}