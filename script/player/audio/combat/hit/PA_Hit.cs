using Godot;

[GlobalClass]
public partial class PA_Hit : Node
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private PA_Sound _criticalHit;
    [Export] private PA_Sound _kill;
    [Export] private PA_Sound _meatHit;
    [Export] private PA_Sound _barrierHit;
    [Export] private PA_Sound _armorHit;

    public override void _Ready()
    {
        _weaponsHandler.Hit += HandleHit;
    }

    public void HandleHit(object sender, HitEventArgs hit)
    {
        if (hit.Missed)
            return;

        if (hit.Killed)
            _kill?.PlaySound();

        if (!hit.OverrideBodyPart && hit.HurtBox.BodyPart == GC_BodyPart.Head)
            _criticalHit?.PlaySound();
        
        GC_Health layer = hit.SenderLayer;

        if (layer is GC_Barrier)
            _barrierHit.PlaySound();
        else if (layer is GC_Armor)
            _armorHit.PlaySound();
        else
            _meatHit.PlaySound();
    }
}