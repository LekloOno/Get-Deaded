using Godot;

/// Provides methods to avoid physics conflicts
public static class PHX_Checks
{
    private const float GROUND_MARGIN = 0.01f;
    public static bool CanUncrouch(PhysicsDirectSpaceState3D spaceState, CollisionObject3D current, CapsuleShape3D originalShape)
    {
        PhysicsShapeQueryParameters3D query = new();
        
        query.SetShape(originalShape);

        Transform3D transform = current.Transform;
        transform.Origin += Vector3.Up * GROUND_MARGIN;
        query.Transform = transform;

        query.CollisionMask = current.CollisionLayer;

        var result = spaceState.IntersectShape(query, 1);
        return result.Count == 0;
    }
}