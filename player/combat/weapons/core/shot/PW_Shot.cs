using System;
using Godot;

[GlobalClass]
public abstract partial class PW_Shot : Resource
{
    [Export] protected GC_Hit _hitData;
    [Export] private Vector3 _originOffset = Vector3.Zero;
    [Export] private Vector3 _directionOffset = Vector3.Zero;


    public EventHandler<ShotHitEventArgs> Hit;

    public void Initialize() => _hitData.InitializeModifiers();

    public void Shoot(Node3D manager, Vector3 origin, Vector3 direction) =>
        HandleShoot(manager, origin + _originOffset, direction + _directionOffset);

    public abstract void HandleShoot(Node3D manager, Vector3 origin, Vector3 direction);
}