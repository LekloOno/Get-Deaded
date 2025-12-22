using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PM_WallJump : PM_Action
{
    
    [Export] private PI_Jump _jumpInput;
    [Export] private PM_Controller _controller;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_LedgeClimb _ledgeClimb;
    [Export] private RayCast3D _wallCastLow;
    [Export] private RayCast3D _wallCastHigh;
    [Export(PropertyHint.Range, "0.0, 10.0")] private float _strength = 5f;
    [Export(PropertyHint.Range, "0.0, 10.0")] private float _boost = 1f;
    [Export(PropertyHint.Range, "0.0, 10.0")] private float _minSpeedInWall = 1.5f;
    [Export(PropertyHint.Range, "0.0,  1.0")] private float _minBounceRatio = 0.6f;
    // The velocity coefficient when straight facing the wall. The more you're facing the wall, the less speed you will keep.

    public Vector3 WallJump(Vector3 velocity)
    {
        if (!_jumpInput.IsBuffered())
            return velocity;            // Nothing to do

        if(_groundState.IsGrounded())
            return _ledgeClimb.LedgeClimb(velocity); // Propagate to Ledge

        Vector3 normal = new();
        if (!IsCollidingWall(ref normal, out GB_IExternalBodyManager entity))
            return _ledgeClimb.LedgeClimb(velocity); // Propagate to Ledge

        if (!IsWall(normal))
            return _ledgeClimb.LedgeClimb(velocity); // Propagate to Ledge


        Vector3 flatVel = MATH_Vector3Ext.Flat(velocity);
        if (_ledgeClimb.CanLedgeClimb() && TooSlow(flatVel, normal))
            return _ledgeClimb.LazyLedgeClimb(velocity); // Force Ledge
        
        Vector3 wallJumpVel = _controller.VelocityCache.UseCacheOr(velocity);
        Vector3 wallFlatVel = MATH_Vector3Ext.Flat(wallJumpVel);

        if (TooSlow(wallFlatVel, normal))
            return _ledgeClimb.LedgeClimb(velocity); // Propagate to Ledge

        _jumpInput.UseBuffer(); 
        return DoWallJump(wallJumpVel, _wallCastLow.GetCollisionNormal(), entity); // Do it !
    }

    private Vector3 DoWallJump(Vector3 velocity, Vector3 normal, GB_IExternalBodyManager entity)
    {
        velocity = velocity.Bounce(normal);

        // Angle ratio -
        // The more the wall jump is performed against the wall, the more speed you lose
        // The more --------------is performed sideways, the less-----
        float angleRatio = velocity.Normalized().Dot(normal.Normalized());
        angleRatio = 1 + angleRatio * _minBounceRatio - angleRatio;
        
        velocity *= angleRatio;

        velocity.Y = _strength;
        OnStart?.Invoke(this, EventArgs.Empty);

        entity?.HandleKnockBack(- MATH_Vector3Ext.Flat(velocity));

        return velocity;
    }

    private bool IsCollidingWall(ref Vector3 normal, out GB_IExternalBodyManager entity)
    {
        entity = null;

        RayCast3D cast = _wallCastLow.IsColliding() ? _wallCastLow
                        : _wallCastHigh.IsColliding() ? _wallCastHigh
                        : null;

        if (cast == null)
            return false;

        normal = cast.GetCollisionNormal();

        if (cast.GetCollider() is GB_IExternalBodyManager a) entity = a;

        return true;
    }

    private bool TooSlow(Vector3 velocity, Vector3 normal) => velocity.Dot(-normal) < _minSpeedInWall;
    private bool IsWall(Vector3 normal) => normal.AngleTo(_controller.UpDirection) > _controller.FloorMaxAngle;
}