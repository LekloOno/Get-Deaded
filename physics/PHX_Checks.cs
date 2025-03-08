using Godot;
using Godot.Collections;

/// Provides methods to avoid physics conflicts
public static class PHX_Checks
{
    public static bool CanUncrouch(PhysicsBody3D current, PB_Scale bodyScale, float upSafeMargin = 0.05f)
    {
        float halfDeltaScale = bodyScale.ScaleDelta/2f;

        Transform3D transform = current.Transform;
        transform.Origin += Vector3.Up * halfDeltaScale;

        Vector3 motion = Vector3.Up * (halfDeltaScale + upSafeMargin);

        return !current.TestMove(transform, motion, maxCollisions: 1, safeMargin: 0.1f, recoveryAsCollision: true);
    }
}