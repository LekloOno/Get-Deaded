using Godot;

[GlobalClass]
public abstract partial class GL_PickableInfos : Resource 
{
    [Export] public PackedScene Model {get; private set;}
    [Export] private PHX_RepulsionField3DData _repulsionData;
    protected abstract GL_PhysicsPickable GeneratePhysicsPickable();
    public GL_PhysicsPickable Generate()
    {
        GL_PhysicsPickable pickable = GeneratePhysicsPickable();
        pickable.TopLevel = true;
        pickable.CollisionLayer = 0x10;
        pickable.AddChild(Model.Instantiate());
        if (_repulsionData == null)
            return pickable;
        
        PHX_RepulsionField3D field = new(_repulsionData);
        pickable.AddChild(field);

        return pickable;
    }
}