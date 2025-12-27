using Godot;

[GlobalClass]
public partial class UI_EscapeMenu : Control
{
    [Export] private bool _pauseGame = true;

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