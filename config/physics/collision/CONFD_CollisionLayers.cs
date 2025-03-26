using Godot;

[GlobalClass]
public partial class CONFD_CollisionLayers : Resource
{
    /// <summary>
    /// Things such as a wall, ground.. Base environment objects, most of the time static.
    /// </summary>
    [Export(PropertyHint.Layers3DPhysics)] public uint Environment {get; private set;}
    /// <summary>
    /// Objects used for Entities interractions with Environment.
    /// The environment collision of the player, ennemies ..
    /// </summary>
    [Export(PropertyHint.Layers3DPhysics)] public uint EnvironmentEntity {get; private set;}
    [Export(PropertyHint.Layers3DPhysics)] public uint EnnemiesHurtBox {get; private set;}
    [Export(PropertyHint.Layers3DPhysics)] public uint PlayerHurtBox {get; private set;}
    [Export(PropertyHint.Layers3DPhysics)] public uint Pickup {get; private set;}
}