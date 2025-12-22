using Godot;

namespace Pew;

[GlobalClass]
public abstract partial class GM_Mover : Node, GM_IMover
{
    public abstract Vector3 Velocity {get;}
    public abstract Vector3 WishDir {get;}
    public abstract Vector2 WalkAxis {get;}
}