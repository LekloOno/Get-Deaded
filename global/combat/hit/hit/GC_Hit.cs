using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GC_Hit : Resource
{
    [Export] private float _damage;
    [Export] private Array<GC_DamageModifier> _inspectorModifiers;
    // We use a "Game designer side" base array, plus a hitModifier "logic side" so we can determine the modifier more intuitively.
    // The indicated multiplier in _inspectorModifiers is as a reference to a basic hurt receiver - the default ones. See examples below.
    // This is because of how multiplier works - i.e. both receiver and sender can specify their own specific multiplier, and they should both have an impact on the final result.
    // --
    // Example -
    // The default headshot multiplier, "hurt-side", is 2.
    // For this hit, the game designer wants to indicate a 2.5 head multiplier, which refers to the multiplier on a default receiver - the hurt-side.
    // The stored multiplier will then be 2.5/2 = 1.25.
    // Thus, the computed damage on a default receiver will be 2 * 1.25 * damage, or 2.5 * damage - the intended multiplier.
    // 
    // However, on a special receiver, which only take a 1.5 head mutliplier
    // The computed damage will be 1.5 * 1.25 * damage, or 1.875 * damage.

    // If the game designer indicates a headshot multiplier of 2 for this weapon, but the base hurt headshot multiplier is already 2, then hitModifier will contain 1 for the head (2/2).
    private GC_HitModifier _hitModifier;


    public GC_Hit() : this(0, new Array<GC_DamageModifier>()) {}
    public GC_Hit(float damage, Array<GC_DamageModifier> inspectorModifiers)
    {
        _damage = damage;
        _inspectorModifiers = null;
        _hitModifier = new(inspectorModifiers);
    }

    public float GetDamage(GC_BodyPart bodyPart) => _hitModifier.GetDamage(bodyPart, _damage);
    public void SendHit(GC_HurtBox hurtBox) => hurtBox.Damage(GetDamage(hurtBox.BodyPart));
}