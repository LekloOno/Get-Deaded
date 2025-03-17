using System;
using Godot;


public abstract partial class PW_Shot : Resource
{
    [Export] protected GC_Hit _hitData;
    public EventHandler<ShotHitEventArgs> Hit;

    public abstract void Shoot(Vector3 origin, Vector3 direction);
}