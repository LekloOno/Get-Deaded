using Godot;

[GlobalClass]
public partial class UI_EscapeMenu : Control
{
    [Export] private PIM_Arena _arenaMap;
    [Export] private bool _pauseGame = true;

    public override void _Ready()
    {
        _arenaMap.OnMenuOpen += Open;
        _arenaMap.OnMenuClose += Close;
    }

    public void Open()
    {
        Show();

        if (_pauseGame)
            GetTree().Paused = true;
    }

    public void Close()
    {
        if (_pauseGame)
            GetTree().Paused = false;

        Hide();
    }
}