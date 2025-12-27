using Godot;

[GlobalClass]
public partial class UI_2DSpaceMenu : Node
{
    [Export] private UI_EscapeMenu _menu;
    [Export] private PIM_Arena _arenaMap;
    [Export] private PC_Control _cameraControl;
    private Input.MouseModeEnum _prevMouseMode;
    private ProcessModeEnum _prevCameraMode;

    public override void _Ready()
    {
        _arenaMap.OnMenuOpen += Open;
        _arenaMap.OnMenuClose += Close;
        _prevMouseMode = Input.MouseMode;
    }

    public void Open()
    {
        _prevCameraMode = _cameraControl.ProcessMode;
        _cameraControl.ProcessMode = ProcessModeEnum.Pausable;

        _prevMouseMode = Input.MouseMode;
        Input.WarpMouse(GetViewport().GetVisibleRect().Size / 2f);
        Input.MouseMode = Input.MouseModeEnum.Visible;
        _menu.Open();
    }

    public void Close()
    {
        _cameraControl.ProcessMode = _prevCameraMode;
        
        _menu.Close();
        Input.MouseMode = _prevMouseMode;
        _cameraControl.SetProcessUnhandledInput(true);
    }
}