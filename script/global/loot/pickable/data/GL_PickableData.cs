using Godot;

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
        pickable.CollisionLayer = 0x10;
        pickable.CollisionMask = 1;
        pickable.AddChild(Model.Instantiate());
        if (_repulsionData == null)
            return pickable;
        
        PHX_RepulsionField3D field = new(_repulsionData);
        pickable.AddChild(field);

        return pickable;
    }
}