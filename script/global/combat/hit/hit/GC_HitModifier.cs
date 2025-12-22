using System.Collections.Generic;
using Godot;

public class GC_HitModifier
{
    private readonly Dictionary<GC_BodyPart, float> _modifiers;

    public GC_HitModifier(Godot.Collections.Array<GC_DamageModifier> damageModifiers)
    {
        _modifiers = new();

        foreach(GC_DamageModifier damageModifier in damageModifiers)
            _modifiers.TryAdd(damageModifier.BodyPart, GC_HurtBox.RealHitModifier(damageModifier));
    }

    public float GetDamage(GC_BodyPart bodyPart, float damage)
    {
        if (_modifiers.TryGetValue(bodyPart, out float modifier))
            return damage * modifier;
        
        return damage;
    }
}