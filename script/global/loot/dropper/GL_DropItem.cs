using Godot;

[GlobalClass]
public partial class GL_DropItem : Resource
{
    [Export] private float _dropRate;
    [Export] private GL_PickableData _item;
    [Export] private float _horizontalDamp;

    // Use a seed to avoid instantiation of random for each drop item.
    public bool TryDrop(float seed, out GL_PhysicsPickable pickable)
    {
        if (seed <= _dropRate)
        {
            pickable = _item.GeneratePhysics(_horizontalDamp);
            return true;
        }
        
        pickable = null;
        return false;
    }
}