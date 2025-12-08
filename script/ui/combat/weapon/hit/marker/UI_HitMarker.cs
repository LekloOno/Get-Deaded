using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UI_HitMarker : Control
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private UI_KillMarker _killMarker;
    [Export] private UI_DamageMarker _damageMarker;
    [Export] private UI_KillSkullManager _killSkull;

    public override void _Ready()
    {
        _weaponsHandler.Hit += HandleHit;
    }

    public void HandleHit(object sender, ShotHitEventArgs e)
    {
        if (e.HurtBox == null || e.Target == null)
            return;
        
        if (e.Killed)
        {
            _killMarker.StartAnim();
            _killSkull.PopSkull(e.HurtBox.BodyPart, e.OverrideBodyPart);
            return;
        }

        _damageMarker.StartAnim(e);
    }
}