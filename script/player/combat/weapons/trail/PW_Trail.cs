using Godot;

/// <summary>
/// Handles visual trail shooting.
/// <para>
/// Its GlobalPosition serves as the shooting origin position.<br/>
/// You can offset it from its Shot parent to indicate where the "barrel" of the weapon's model is.
/// </para>
/// </summary>

// Icon credits - Lorc - under CC BY 3.0 - https://lorcblog.blogspot.com/ - https://game-icons.net/1x1/lorc/sinusoidal-beam.html
[GlobalClass, Icon("res://gd_icons/weapon_system/trail_icon.svg")]
public partial class PW_Trail : WeaponComponent
{
    [Export] protected Godot.Collections.Array<VFX_Trail> _trails;

    public override void _Ready()
    {
        foreach (VFX_Trail trail in _trails)
            trail.Preload(this, 1);
    }


    public void Shoot(Vector3 hitPosition)
    {
        foreach (VFX_Trail trail in _trails)
            trail.Shoot(this, GlobalPosition, hitPosition);
    }
}