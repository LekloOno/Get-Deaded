using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_EscapeMenu : Control
{
    [Export] private bool _pauseGame = true;
    [Export] private Control _menus;
    [Export] private Control _baseMenu;
    private Stack<Control> _menuStack = [];

    public override void _Ready()
    {
        foreach (Node child in _menus.GetChildren())
            if (child is Control control)
                control.Visible = false;
    }

    public void Open()
    {
        Show();
        Enter(_baseMenu);

        if (_pauseGame)
            GetTree().Paused = true;
    }

    public void Close()
    {
        if (_pauseGame)
            GetTree().Paused = false;

        Hide();
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
    public bool ExitCurrent()
    {
        Control current = _menuStack.Pop();
        current.Hide();

        bool stillInMenu = _menuStack.TryPeek(out Control next); 
        
        if (stillInMenu)
            next.Show();
        else
            Close();

        return stillInMenu;
    }
}