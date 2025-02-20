using System;
using Godot;

public abstract partial class PM_Action : Node
{
    public EventHandler OnStart;
    public EventHandler OnStop;
}