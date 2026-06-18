using Godot;

public partial class UI_Loading : Control
{
    [Export] private Control _icon = null!;
    [Export] private float _degreesPerSec = 100f;

    public override void _Ready()
    {
        StopLoading();
    }

    public void StartLoading()
    {
        Visible = true;
        SetProcess(true);
    }

    public void StopLoading()
    {
        Visible = false;
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        _icon.RotationDegrees += (float) delta * _degreesPerSec;
    }
}