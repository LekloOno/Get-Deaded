using Godot;

namespace Pew;

public static class PHX_MovementPhysics
{
    static public Vector3 Acceleration(float maxSpeedBase, float maxAccelBase, Vector3 velocity, Vector3 direction, float deltaTime)
    {
    /*  Returns the Accelerated Vector to be added to the current velocity vector of an entity.
    */
        float currentSpeed = velocity.Dot(direction);
        float accel = Mathf.Max(Mathf.Min(maxSpeedBase - currentSpeed, maxAccelBase*maxSpeedBase*deltaTime),0);
        return accel * direction;
    }
}