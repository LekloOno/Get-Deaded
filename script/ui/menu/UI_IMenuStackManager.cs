using System;
using Godot;

public interface UI_IMenuStackManager
{
    public event Action Exit;
    public void Enter(Control menu);
    public bool ExitCurrent();
}