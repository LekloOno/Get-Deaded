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

    public event Action<GC_HurtBox> HitHurtBox;
    public event Action HitEnvironment;
    private Dictionary<Node, ulong> _hitEntities = [];

    public override void _Ready()
    {
        _sequenceArea.AreaEntered += OnBodyEntered;
        _sequenceArea.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is GC_HurtBox hurtBox)
            RegisterHit(hurtBox);
        else
            RegisterMiss(body);
    }

    private void RegisterHit(GC_HurtBox hurtBox)
    {
        if (Register(hurtBox.Entity))
            HitHurtBox?.Invoke(hurtBox);
    }

    private void RegisterMiss(Node node)
    {
        if (Register(node))
            HitEnvironment?.Invoke();
    }

    private bool Register(Node node)
    {
        if (!CanRegister(node))
            return false;

        _hitEntities[node] = PHX_Time.ScaledTicksMsec;
        return true;
    }

    private bool CanRegister(Node node)
    {
        if (_hitCooldown == 0)
            return true;
        
        if (!_hitEntities.TryGetValue(node, out ulong lastHit))
            return true;

        return CoolDownPassed(lastHit);
    }

    private bool CoolDownPassed(ulong lastHit) => _hitCooldown == 0 || PHX_Time.ScaledTicksMsec - lastHit >= _hitCooldown;


    public void StartSequence() => _sequenceArea.StartSequence();
    public void StopSequence() => _sequenceArea.StopSequence();
}