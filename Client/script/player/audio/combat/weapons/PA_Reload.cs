using GaudioProcessTree;
using Godot;

public partial class PA_Reload : Node3D
{
    [Export] private PW_Weapon? _weapon;
    [Export] private AUD_Sound? _startUnloadSound;
    [Export] private AUD_Sound? _unloadSound;
    [Export] private AUD_Sound? _startInsertSound;
    [Export] private AUD_Sound? _insertSound;
    [Export] private AUD_Sound? _startChamberSound;
    [Export] private AUD_Sound? _chamberSound;
    [Export] private AUD_Sound? _recoverSound;

    public override void _Ready()
    {
        if (_weapon == null)
        {
            GD.PrintErr("[PA_Reload] missing weapon.");
            return;
        }
        
        if (_weapon.Reloader == null)
        {
            CallDeferred(nameof(_Ready));
            return;
        }

        _weapon.Reloader.Started += OnReloadStarted;

        if (_unloadSound != null)
            _weapon.Reloader.Unloaded += _unloadSound.Play;

        if (_startInsertSound != null)   
            _weapon.Reloader.Unloaded += _startInsertSound.Play;

        if (_insertSound != null)
            _weapon.Reloader.Inserted += _insertSound.Play;

        if (_startChamberSound != null)
            _weapon.Reloader.Inserted += _startChamberSound.Play;

        if (_chamberSound != null)
            _weapon.Reloader.Chambered += _chamberSound.Play;

        if (_recoverSound != null)
            _weapon.Reloader.Recovered += _recoverSound.Play;
    }

    private void OnReloadStarted(PW_ReloadStep prev, PW_ReloadStep current, float time)
    {
        switch (current)
        {
            case PW_ReloadStep.Unload:
                _startUnloadSound?.Play();
                break;

            case PW_ReloadStep.Insert:
                _startInsertSound?.Play();
                break;

            case PW_ReloadStep.Chamber:
                _startChamberSound?.Play();
                break;
        }
    }
}