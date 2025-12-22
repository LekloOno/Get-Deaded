using Godot;

namespace Pew;

public interface GM_IMover
{
    public Vector3 Velocity {get;}
    public Vector3 WishDir {get;}
    public Vector2 WalkAxis {get;}
}