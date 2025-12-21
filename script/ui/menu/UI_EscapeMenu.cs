using Godot;

[GlobalClass]
public partial class UI_EscapeMenu : Control
{
    [Export] private PIM_Arena _arenaMap;

    public override void _Ready()
    {
        _arenaMap.OnMenuOpen += Show;
        _arenaMap.OnMenuClose += Hide;
    }
}