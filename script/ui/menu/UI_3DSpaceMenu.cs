using Godot;

[GlobalClass]
public partial class UI_3DSpaceMenu : Sprite3D
{
    [Export] private UI_EscapeMenu _menu;
    [Export] private Node3D _backgroundDimmer;
    [Export] private PIM_Arena _arenaMap;
    [Export] private Node3D _cameraBiPivot;
    [Export] private Node3D _pivot;
    [Export] private Node3D _spatialUI;

    public override void _Ready()
    {
        _arenaMap.OnMenuOpen += Open;
        _arenaMap.OnMenuClose += Close;
    }

    public void Open()
    {
        Show();
        _backgroundDimmer?.Show();
        _spatialUI?.Hide();
        _pivot.GlobalRotation = _cameraBiPivot.GlobalRotation;
    }

    public void Close()
    {
        Hide();
        _backgroundDimmer?.Hide();
        _spatialUI?.Show();
    }
}