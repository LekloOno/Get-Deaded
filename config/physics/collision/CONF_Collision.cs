using Godot;

namespace Pew;

public partial class CONF_Collision : Node
{
    public static CONF_Collision Instance { get; private set; }

    public CONFD_CollisionLayers CollisionLayers { get; private set; }
    public CONFD_CollisionMasks CollisionMasks { get; private set; }

    public static CONFD_CollisionLayers Layers { get => Instance.CollisionLayers; }
    public static CONFD_CollisionMasks Masks { get => Instance.CollisionMasks; }

    public override void _EnterTree()
    {
        Instance = this;
        CollisionLayers = (CONFD_CollisionLayers) ResourceLoader.Load("res://config/physics/collision/conf_collision_layers.tres");
        CollisionMasks = (CONFD_CollisionMasks) ResourceLoader.Load("res://config/physics/collision/conf_collision_masks.tres");
    }
}