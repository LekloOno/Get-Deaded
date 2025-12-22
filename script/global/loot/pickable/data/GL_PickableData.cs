using Godot;

namespace Pew;

[GlobalClass]
public abstract partial class GL_PickableData : Resource 
{
    [Export] public PackedScene Model {get; private set;}
    [Export] private PHX_RepulsionField3DData _repulsionData;
    protected abstract GL_PhysicsPickable GetPhysicsPickable(float horizontalDamp, float lifeTime);
    public GL_PhysicsPickable GeneratePhysics(float horizontalDamp, float lifeTime)
    {
        GL_PhysicsPickable pickable = GetPhysicsPickable(horizontalDamp, lifeTime);
        pickable.TopLevel = true;
        pickable.CollisionLayer = CONF_Collision.Layers.Pickup;
        pickable.CollisionMask = CONF_Collision.Masks.Environment;
        pickable.AddChild(Model.Instantiate());
        if (_repulsionData == null)
            return pickable;
        
        PHX_RepulsionField3D field = new(_repulsionData);
        pickable.AddChild(field);

        return pickable;
    }
}