using Godot;

[GlobalClass]
public partial class UI_Reloading : TextureProgressBar
{
    [Export] private PW_WeaponsHandler _weaponsHandler;

    private float _reloadTime = 0f;
    private float _elapsedTime = 0f;

    public override void _Ready()
    {
        Visible = false;
        _weaponsHandler.ReloadStarted += OnReload;
        _weaponsHandler.ReloadCancelled += OnReloadCancelled;
    }

    public override void _ExitTree()
    {
        OnReloadCancelled();
    }

    private void OnReloadCancelled()
    {
        Visible = false;
        SetProcess(false);
    }

    public void OnReload(float time)
    {
        Visible = true;
        _reloadTime = time;
        _elapsedTime = 0f;
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        if (PW_WeaponReloader.IgnoreTimeScale)
            _elapsedTime += (float) delta / (float) Engine.TimeScale;
        else
            _elapsedTime += (float) delta;

        Value = _elapsedTime / _reloadTime;

        if (_elapsedTime >= _reloadTime)
            OnReloadCancelled();
    }
}