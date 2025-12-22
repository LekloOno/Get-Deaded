using Godot;

namespace Pew;

[GlobalClass]
public partial class GL_DropItem : Resource
{
    [Export] private float _dropRate;
    [Export] private GL_PickableData _item;
    [Export] private float _horizontalDamp;
    [Export] private float _lifeTime;

    // Use a seed to avoid instantiation of random for each drop item.
    public bool TryDrop(float seed, out GL_PhysicsPickable pickable)
    {
        if (seed <= _dropRate)
        {
            pickable = _item.GeneratePhysics(_horizontalDamp, _lifeTime);
            return true;
        }
        
        pickable = null;
        return false;
    }
}