using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class GC_SequenceHitBox : Node
{
    [Export] private PHX_SequenceArea3D _sequenceArea;
    /// <summary>
    /// If a combat entity enters in the same sequence hit box multiple times in less than this amount of time (milli-sec), it will be ignored.
    /// </summary>
    [Export] private ulong _hitCooldown;
    public uint CollisionMask
    {
        get => _sequenceArea.CollisionMask;
        set => _sequenceArea.CollisionMask = value;
    }

    
    public uint CollisionLayer
    {
        get => _sequenceArea.CollisionLayer;
        set => _sequenceArea.CollisionLayer = value;
    }

    public event Action<GC_HurtBox> HitEntity;
    private Dictionary<GE_CombatEntity, ulong> _hitEntities = [];

    public override void _Ready()
    {
        _sequenceArea.AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area3D area)
    {
        if (area is not GC_HurtBox hurtBox)
            return;

        if (CanRegister(hurtBox))
            RegisterHit(hurtBox);
    }

    private bool CanRegister(GC_HurtBox hurtBox)
    {
        if (_hitCooldown == 0)
            return true;
        
        if (!_hitEntities.TryGetValue(hurtBox.Entity, out ulong lastHit))
            return true;

        return CoolDownPassed(lastHit);
    }

    private bool CoolDownPassed(ulong lastHit) => _hitCooldown == 0 || PHX_Time.ScaledTicksMsec - lastHit >= _hitCooldown;

    private void RegisterHit(GC_HurtBox hurtBox)
    {
        _hitEntities[hurtBox.Entity] = PHX_Time.ScaledTicksMsec;
        HitEntity?.Invoke(hurtBox);
    }

    public void StartSequence() => _sequenceArea.StartSequence();
    public void StopSequence() => _sequenceArea.StopSequence();
}