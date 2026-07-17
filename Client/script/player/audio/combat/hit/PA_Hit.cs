using GaudioProcessTree;
using Godot;

[GlobalClass]
public partial class PA_Hit : Node
{
    [Export] private PW_WeaponsHandler _weaponsHandler = null!;
    [Export] private AUD_Sound _criticalHit = null!;
    [Export] private AUD_Sound _criticalDing = null!;
    [Export] private AUD_Sound _kill = null!;
    [Export] private AUD_Sound _criticalKill = null!;
    [Export] private AUD_Sound _meatHit = null!;
    [Export] private AUD_Sound _barrierHit = null!;
    [Export] private AUD_Sound _armorHit = null!;
    [Export] private ulong _minimumDelay = 45;
    [Export] private ulong _criticalMinimumDelay = 100;
    [Export] private bool _volumeDamageScale = true;
    [Export] private float _minimumVolume = -20f;
    [Export] private float _maximumVolume = 0f;
    [Export] private float _damageDispersion = 150f; 
    [Export] private float _minimumDingPitch = 0.4f;
    [Export] private float _maximumDingPitch = 1f;
    [Export] private uint _maximumKillChain = 8;
    [Export] private float _pitchPerKill = 0.1f;
    private ulong _lastBarrier = 0;
    private ulong _lastArmor = 0;
    private ulong _lastMeat = 0;
    private ulong _lastCritical = 0;

    private ulong _lastKill = 0;
    private uint _chain = 0;

    public override void _Ready()
    {
        _weaponsHandler.Hit += HandleHit;
    }

    public void HandleHit(object sender, HitEventArgs hit)
    {
        if (hit.Missed)
            return;

        if (hit.Killed)
        {
            if (hit.Critical)
                _criticalKill.Play();
                
            PlayKill();
        }

        if (!hit.OverrideBodyPart && hit.HurtBox.BodyPart == GC_BodyPart.Head)
        {
            PitchDing(hit.TotalDamage);
            PlayHit(_criticalHit, ref _lastCritical, _criticalMinimumDelay, hit.TotalDamage);
        }
        
        GC_Health layer = hit.SenderLayer;

        if (layer is GC_Barrier)
            PlayHit(_barrierHit, ref _lastBarrier, _minimumDelay, hit.TotalDamage);
        else if (layer is GC_Armor)
            PlayHit(_armorHit, ref _lastArmor, _minimumDelay, hit.TotalDamage);
        else
            PlayHit(_meatHit, ref _lastMeat, _minimumDelay, hit.TotalDamage);
    }

    private void PlayKill()
    {
        ulong now = PHX_Time.ScaledTicksMsec;
        if (now - _lastKill > UI_KillSkullManager.FadeTime * 1000f)
            _chain = 0;
        else if (_chain < _maximumKillChain)
            _chain ++;

        _lastKill = now;

        float pitch = Mathf.Pow(1f + _pitchPerKill, _chain);
        
        _kill.RelativePitchScale = pitch;
        _kill.Play();
    }

    private void PitchDing(float damage)
    {
        float pitchRatio = 1 - Mathf.Tanh(damage/_damageDispersion);
        _criticalDing.RelativePitchScale = Mathf.Lerp(_minimumDingPitch, _maximumDingPitch, pitchRatio);
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