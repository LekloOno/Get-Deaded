using Godot;

// A child of a HealthManager.
// It could be a head, a chest, leg, etc.
// It's the physical part of the health system.
[GlobalClass]
public partial class GC_HurtBox : Area3D
{
    [Export] public GC_BodyPart BodyPart {get; private set;} = GC_BodyPart.Chest;
    [Export] private bool _useSpecialModifier = false;
    [Export] private float _modifier = 1f;
    [Export] public GC_HealthManager HealthManager {get; private set;}
    [Export] public GpuParticles3D _damageSplatter;
    
    public override void _Ready()
    {
        if (!_useSpecialModifier) _modifier = CONF_BodyModifiers.GetDefaultModifier(BodyPart);
    }

    public bool Damage(GC_IHitDealer hitDealer, out float takenDamage, out float overflow) => HealthManager.Damage(hitDealer, hitDealer.HitData.GetDamage(BodyPart) * _modifier, out takenDamage, out overflow);
    public float Heal(float heal) => HealthManager.Heal(heal);
    public bool TriggerDamageParticles(Vector3 hitPosition, Vector3 from)
    {
        if (_damageSplatter == null)
            return false;

        _damageSplatter.GlobalPosition = hitPosition;
        _damageSplatter.LookAt(from);
        _damageSplatter.Emitting = true;
        return true;
    }

    public void HandleKnockBack(Vector3 force) => HealthManager.HandleKnockBack(force);
    public static float RealHitModifier(GC_DamageModifier damageModifier) => damageModifier/CONF_BodyModifiers.GetDefaultModifier(damageModifier.BodyPart);

}