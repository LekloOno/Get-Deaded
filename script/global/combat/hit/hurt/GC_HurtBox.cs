using System;
using Godot;


/// <summary>
/// A child of a HealthManager. <br/>
/// It could be a head, a chest, leg, etc.
/// It's the physical part of the health system.
/// </summary>
[GlobalClass]
public partial class GC_HurtBox : Area3D
{
    [Export] public GC_BodyPart BodyPart {get; private set;} = GC_BodyPart.Chest;
    [Export] private bool _useSpecialModifier = false;
    [Export] private float _modifier = 1f;
    [Export] public GpuParticles3D DamageSplatter;
    [Export] public GE_CombatEntity Entity {get; private set;}
    public static float BackAngle = 135;
    public PHX_ActiveRagdollBone RagdollBone {get; private set;}
    
    public override void _Ready()
    {
        // Defaults out the damage modifier of the hurtbox.
        if (!_useSpecialModifier)
            _modifier = CONF_BodyModifiers.GetDefaultModifier(BodyPart);
    }

    // Instead of a direct set access, to make it explicity it should not be modified exepct for initialization.
    public void InitRagdollBone(PHX_ActiveRagdollBone bone) => RagdollBone = bone;

    public static float RealHitModifier(GC_DamageModifier damageModifier) =>
        damageModifier / CONF_BodyModifiers.GetDefaultModifier(damageModifier.BodyPart);

    public bool Damage(GC_IHitDealer hitDealer, out float takenDamage, out float overflow, out GC_Health deepest, float subHitSize = 1f)
    {
        float expectedDamage = hitDealer.HitData.GetDamage(BodyPart);
        expectedDamage *= _modifier;
        expectedDamage *= subHitSize;
        expectedDamage *= DirMultiplier(hitDealer);

        return Entity.HealthManager.Damage(hitDealer, expectedDamage, out takenDamage, out overflow, out deepest);
    }

    private float DirMultiplier(GC_IHitDealer hitDealer)
    {
        float multiplier = hitDealer.HitData.BackModifier;
        
        if (multiplier == 1f)
            return 1f;
        if (IsHittingFront(hitDealer))
            return 1f;

        return multiplier;
    }

    private bool IsHittingFront(GC_IHitDealer hitDealer)
    {
        Vector3 selfDir = -Entity.Body.GlobalTransform.Basis.Z;
        Vector3 hitDealerPos = hitDealer.OwnerEntity.Body.GlobalTransform.Origin;
        float hitAngle = MATH_Vector3Ext.FlatAngle(selfDir, hitDealerPos);
        
        return hitAngle < BackAngle;
    }

    public float Heal(float heal) => Entity.HealthManager.Heal(heal);

    public bool TriggerDamageParticles(Vector3 hitPosition, Vector3 from)
    {
        if (DamageSplatter == null)
            return false;

        DamageSplatter.GlobalPosition = hitPosition;
        DamageSplatter.LookAt(from);
        DamageSplatter.Emitting = true;
        return true;
    }

    public void HandleKnockBack(Vector3 force) =>
        Entity.HealthManager.HandleKnockBack(force);

    public HitEventArgs HandleHit(
        GE_IActiveCombatEntity author,
        GC_IHitDealer hitDealer,
        Vector3 hitPosition,
        Vector3 from,
        Vector3? localKnockback,    // used for ragdoll physics
        Vector3? globalKnockBack,   // Global KnockBack, actually influences the entity position
        bool overrideBodyPart = false,
        float subHitSize = 1f       // used to emulate sub-tick continuous fire. 
    ) {
        bool killed = Damage(
            hitDealer,
            out float takenDamage,
            out float overflow,
            out GC_Health deepest,
            subHitSize
        );

        if (globalKnockBack is Vector3 knockBack)
            HandleKnockBack(knockBack);

        if (killed && RagdollBone is PHX_ActiveRagdollBone bone)
        {
            if (localKnockback is Vector3 localImpulse)
                bone.Hit(localImpulse, hitPosition);
            
            if (globalKnockBack is Vector3 globalImpulse)
                bone.GlobalHit(globalImpulse, hitPosition);
        }

        TriggerDamageParticles(hitPosition, from); // Experimental from position, to check.

        return new(
            Entity, deepest, this,           // Target infos
            takenDamage, killed,                    // Hit infos
            hitDealer, author,                      // Author infos
            overflow, overrideBodyPart,             // Optional infos
            false, subHitSize                       // Optional infos
        );
    }
}