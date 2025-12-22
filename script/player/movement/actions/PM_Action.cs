using System;
using Godot;

namespace Pew;

public abstract partial class PM_Action : Node
{
    public EventHandler OnStart;
    public EventHandler OnStop;
}