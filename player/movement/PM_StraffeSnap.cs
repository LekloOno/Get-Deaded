using Godot;

[GlobalClass]
public partial class PM_StraffeSnap : Node
{
    [Export] public PS_Grounded GroundState {get; private set;}
    [Export(PropertyHint.Range, "0.0, 3.0")] public float SnapThreshold;

    public Vector3 Snap(Vector3 velocity, Vector3 prevVelocity)
    {
        if (GroundState.IsGrounded() && velocity.Length() < prevVelocity.Length() && velocity.Length() < SnapThreshold)
            return Vector3.Zero;
        
        return velocity;
    }
}