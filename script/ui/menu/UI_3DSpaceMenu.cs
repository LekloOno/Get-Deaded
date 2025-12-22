using Godot;

namespace Pew;

[GlobalClass]
public partial class UI_3DSpaceMenu : Sprite3D
{
    [Export] private UI_EscapeMenu _menu;
    [Export] private Node3D _backgroundDimmer;
    [Export] private PIM_Arena _arenaMap;
    [Export] private Node3D _cameraBiPivot;
    [Export] private Node3D _pivot;
    [Export] private Node3D _combatSpatialUI;
    [Export] private CanvasLayer _combatControlUI;
    [Export] private CanvasLayer _menuCrossHair;

    public override void _Ready()
    {
        _arenaMap.OnMenuOpen += Open;
        _arenaMap.OnMenuClose += Close;
    }

    public void Open()
    {
        Show();
        _backgroundDimmer?.Show();
        _combatSpatialUI?.Hide();
        _combatControlUI?.Hide();
        _menuCrossHair?.Show();
        _pivot.GlobalRotation = _cameraBiPivot.GlobalRotation;
    }

    public void Close()
    {
        Hide();
        _backgroundDimmer?.Hide();
        _combatSpatialUI?.Show();
        _combatControlUI?.Show();
        _menuCrossHair?.Hide();
    }
}