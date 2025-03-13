using Godot;
using Godot.Collections;

// A child of a HealthManager.
// It could be a head, a chest, leg, etc.
// It's the physical part of the health system.
[GlobalClass]
public partial class GC_HurtBox : Area3D
{
    [Export] public GC_BodyPart BodyPart {get; private set;} = GC_BodyPart.Chest;
    [Export] private bool _useSpecialModifier = false;
    [Export] private float _modifier = 1f;
    [Export] private GC_HealthManager _healthManager;
    
    public override void _Ready()
    {
        if (!_useSpecialModifier) _modifier = CONF_BodyModifiers.GetDefaultModifier(BodyPart);
    }

    public bool Damage(float damage) => _healthManager.Damage(damage * _modifier);
    public float Heal(float heal) => _healthManager.Heal(heal);
    
    public static float RealHitModifier(GC_DamageModifier damageModifier) => damageModifier/CONF_BodyModifiers.GetDefaultModifier(damageModifier.BodyPart);

}