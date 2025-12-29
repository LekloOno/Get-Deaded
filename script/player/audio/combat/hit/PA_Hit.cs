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
            _criticalHit?.Play();
        
        GC_Health layer = hit.SenderLayer;

        if (layer is GC_Barrier)
            _barrierHit.Play();
        else if (layer is GC_Armor)
            _armorHit.Play();
        else
            _meatHit.Play();
    }
}