using Godot;

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
    [Export] private bool _useMouseCursor = false;
    private Input.MouseModeEnum _prevMouseMode;

    public override void _Ready()
    {
        _arenaMap.OnMenuOpen += Open;
        _arenaMap.OnMenuClose += Close;
        _prevMouseMode = Input.MouseMode;

        _menu.Exit += Resume;
    }

    public void Open()
    {
        if (_useMouseCursor)
        {
            _prevMouseMode = Input.MouseMode;
            Input.WarpMouse(GetViewport().GetVisibleRect().Size / 2f);
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        Show();
        _backgroundDimmer?.Show();
        _combatSpatialUI?.Hide();
        _combatControlUI?.Hide();
        _menuCrossHair?.Show();
        _pivot.GlobalRotation = _cameraBiPivot.GlobalRotation;

        _menu.Open();
    }

    public void Close()
    {
        if (_menu.ExitCurrent())
            return;

        Resume();
    }

    public void Resume()
    {
        if (_useMouseCursor)
            Input.MouseMode = _prevMouseMode;
            
        Hide();
        _backgroundDimmer?.Hide();
        _combatSpatialUI?.Show();
        _combatControlUI?.Show();
        _menuCrossHair?.Hide();
    }
}