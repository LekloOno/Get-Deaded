using Godot;

[GlobalClass]
public partial class UI_3DSpaceMenu : Sprite3D
{
    [Export] private Node3D _backgroundDimmer;
    [Export] private Node3D _cameraBiPivot;
    [Export] private Node3D _pivot;
    [Export] private Node3D _combatSpatialUI;
    [Export] private CanvasLayer _combatControlUI;
    [Export] private CanvasLayer _menuCrossHair;
    [Export] private bool _useMouseCursor = false;
    private Input.MouseModeEnum _prevMouseMode;

    public override void _Ready()
    {
        _prevMouseMode = Input.MouseMode;
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
    }

    public void Close()
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