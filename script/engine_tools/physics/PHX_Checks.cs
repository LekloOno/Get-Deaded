using Godot;

/// Provides methods to avoid physics conflicts
public static class PHX_Checks
{
    public static bool CanUncrouch(PhysicsBody3D current, PHX_BodyScale bodyScale, float upSafeMargin = 0.05f)
    {
        float halfDeltaScale = bodyScale.ScaleDelta/2f;

        Transform3D transform = current.Transform;
        transform.Origin += Vector3.Up * halfDeltaScale;

        Vector3 motion = Vector3.Up * (halfDeltaScale + upSafeMargin);

        return !current.TestMove(transform, motion, maxCollisions: 1, safeMargin: 0.1f, recoveryAsCollision: true);
    }

    public static bool CanMoveForward(PhysicsBody3D current, BoxShape3D shape, Node3D pivot, float amount, out KinematicCollision3D result, float feetMargin = 0f)
    {
        Vector3 feetMarginVector = new(0f, feetMargin, 0f);
        Transform3D transform = current.Transform;
        Vector3 motion = -pivot.GlobalBasis.Z * amount;

        KinematicCollision3D localResult = new();

        Vector3 initSize = shape.Size;
        if (feetMargin != 0)
        {
            shape.Size -= feetMarginVector;
            transform.Origin += feetMarginVector/2f;
        }

        bool canMove = !current.TestMove(transform, motion, localResult);
        result = localResult;

        shape.Size = initSize;

        return canMove;
    }

    public static bool CanLedgeClimb(PhysicsBody3D current, BoxShape3D shape, Node3D pivot, float amount, float minHeight, ShapeCast3D headCast, out KinematicCollision3D result)
    {
        Vector3 motion = -pivot.GlobalBasis.Z * amount;
        headCast.TargetPosition = motion;
        headCast.ForceShapecastUpdate();

        result = new();

        if (headCast.IsColliding())
            return false;

        return !CanMoveForward(current, shape, pivot, amount, out result, minHeight);
    }
}