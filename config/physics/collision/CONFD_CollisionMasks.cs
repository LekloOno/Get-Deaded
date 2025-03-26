using Godot;

[GlobalClass]
public partial class CONFD_CollisionMasks : Resource
{
    [Export(PropertyHint.Layers3DPhysics)] public uint HitBox {get; private set;}
    public uint HitScan {get => CONF_Collision.Layers.Environment | CONF_Collision.Layers.EnnemiesHurtBox;}
    public uint Environment {get => CONF_Collision.Layers.Environment | CONF_Collision.Layers.EnvironmentEntity;}
    public uint HurtBox {get => 0;}
    public uint Picker {get => CONF_Collision.Layers.Pickup;}
}