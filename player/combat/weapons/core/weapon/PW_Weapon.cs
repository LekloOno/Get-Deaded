using System;
using Godot;

[GlobalClass]
public abstract partial class PW_Weapon : Resource
{
    [Export] public float SwitchInTime {get; private set;}
    [Export] public float SwitchOutTime {get; private set;}
    [Export] public float MoveSpeedModifier {get; private set;} = 0f;        // An additive modifier to set. - is a malus + is a bonus
    protected Camera3D _camera;
    protected Node3D _sight;
    protected Node3D _barel;
    public EventHandler<ShotHitEventArgs> Hit;

    public void Initialize(Camera3D camera, Node3D sight, Node3D barel)
    {
        _camera = camera;
        _sight = sight;
        _barel = barel;
        WeaponInitialize();
    }   

    public virtual void WeaponInitialize() {}

    public abstract void PrimaryDown();
    public abstract void PrimaryUp();

    public abstract void SecondaryDown();
    public abstract void SecondaryUp();
}