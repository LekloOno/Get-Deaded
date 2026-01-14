using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_EscapeMenu : Control, UI_IMenuStackManager
{
    [Export] private bool _pauseGame = true;
    [Export] private Control _baseMenu;
    private Stack<Control> _menuStack = [];
    [Signal]
    public delegate void ClosedEventHandler();
    [Signal]
    public delegate void OpenedEventHandler();

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
            if (child is Control control)
                control.Visible = false;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_cancel"))
            return;
        
        if (Visible)
            ExitCurrent();
        else
            Open();
    }

    public void Open()
    {
        Show();
        Enter(_baseMenu);

        if (_pauseGame)
            GetTree().Paused = true;

        EmitSignal(SignalName.Opened);
    }

    public void Close()
    {
        if (_pauseGame)
            GetTree().Paused = false;

        Hide();
        EmitSignal(SignalName.Closed);
    }

    public void Enter(Control menu)
    {
        _menuStack.Push(menu);
        menu.Show();
    }

    /// <summary>
    /// Exit the current menu, and returns true if it is still in a stacked menu.
    /// </summary>
    /// <returns></returns>
    public void ExitCurrent()
    {
        Control current = _menuStack.Pop();
        current.Hide();

        if (_menuStack.TryPeek(out Control next))
            next.Show();
        else
            Close();
    }
}