using GaudioProcessTree;
using Godot;

[GlobalClass]
public partial class PA_Hurt : Node
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private AUD_Sound _meatHit;
    [Export] private AUD_Sound _shieldHit;
    [Export] private ulong _minimumDelay = 45;
    [Export] private ulong _criticalMinimumDelay = 100;
    [Export] private bool _volumeDamageScale = true;
    [Export] private float _minimumVolume = -20f;
    [Export] private float _maximumVolume = 0f;
    [Export] private float _damageDispersion = 150f; 
    private ulong _lastShield = 0;
    private ulong _lastMeat = 0;

    public override void _Ready()
    {
        _healthManager.OnDamage += HandleHurt;
    }

    private void HandleHurt(GC_Health senderLayer, DamageEventArgs e)
    {
        if (senderLayer is GC_Shield)
            PlayHurt(_shieldHit, ref _lastShield, _minimumDelay, e.Amount);
        else    
            PlayHurt(_meatHit, ref _lastMeat, _minimumDelay, e.Amount);
    }

    private void PlayHurt(AUD_Sound type, ref ulong lastProc, ulong delay, float damage)
    {
        ulong now = PHX_Time.ScaledTicksMsec;
        if (now - lastProc < delay)
            return;

        if (_volumeDamageScale)
        {
            float volumeRatio = Mathf.Tanh(damage/_damageDispersion);
            type.RelativeVolumeDb = Mathf.Lerp(_minimumVolume, _maximumVolume, volumeRatio);
        }

        type.Play();
        lastProc = now;
    }
}