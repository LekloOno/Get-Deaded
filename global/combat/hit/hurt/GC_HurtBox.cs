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
        if (!_useSpecialModifier) _modifier = Default(BodyPart);
    }

    public override void _PhysicsProcess(double delta)
    {
        // To implement
    }
    public override void _Process(double delta)
    {
        // To implement
    }

    public bool Damage(float damage) => _healthManager.Damage(damage * _modifier);
    
    public static float RealHitModifier(GC_DamageModifier damageModifier) => damageModifier/Default(damageModifier.BodyPart);
    public static float Default(GC_BodyPart bodyPart)
    {
        return bodyPart switch {
            GC_BodyPart.Head => 1f,
            GC_BodyPart.Chest => 2f,
            _ => 1f,
        };
    }

}