using Godot;

[GlobalClass]
public partial class PA_Hit : Node
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private AUD_Sound _criticalHit;
    [Export] private AUD_Sound _kill;
    [Export] private AUD_Sound _meatHit;
    [Export] private AUD_Sound _barrierHit;
    [Export] private AUD_Sound _armorHit;
    [Export] private ulong _minimumDelay = 45;
    [Export] private ulong _criticalMinimumDelay = 100;
    [Export] private bool _volumeDamageScale = true;
    [Export] private float _minimumVolume = -20f;
    [Export] private float _maximumVolume = 0f;
    [Export] private float _damageDispersion = 150f; 
    private ulong _lastBarrier = 0;
    private ulong _lastArmor = 0;
    private ulong _lastMeat = 0;
    private ulong _lastCritical = 0;

    public override void _Ready()
    {
        _weaponsHandler.Hit += HandleHit;
    }

    public void HandleHit(object sender, HitEventArgs hit)
    {
        if (hit.Missed)
            return;

        if (hit.Killed)
            _kill?.Play();

        if (!hit.OverrideBodyPart && hit.HurtBox.BodyPart == GC_BodyPart.Head)
            PlayHit(_criticalHit, ref _lastCritical, _criticalMinimumDelay, hit.Damage);
        
        GC_Health layer = hit.SenderLayer;

        if (layer is GC_Barrier)
            PlayHit(_barrierHit, ref _lastBarrier, _minimumDelay, hit.Damage);
        else if (layer is GC_Armor)
            PlayHit(_armorHit, ref _lastArmor, _minimumDelay, hit.Damage);
        else
            PlayHit(_meatHit, ref _lastMeat, _minimumDelay, hit.Damage);
    }

    private void PlayHit(AUD_Sound type, ref ulong lastProc, ulong delay, float damage)
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