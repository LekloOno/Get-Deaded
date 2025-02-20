using System;
using System.ComponentModel.DataAnnotations.Schema;
using Godot;

[GlobalClass]
public partial class PM_LedgeClimb : PM_Action
{
    [Export] private PI_Jump _jumpInput;
    [Export] private PI_CrouchDispatcher _crouchInput;
    [Export] private PI_Walk _walkInput;
    [Export] private PM_Controller _controller;
    [Export] private PM_Dash _dash;
    [Export] private PM_Jump _jump;
    [Export] private RayCast3D _headCast;
    [Export] private RayCast3D _chestCast;
    [Export] private RayCast3D _footCast;

    [Export(PropertyHint.Range, "0.0, 2.0")] private float _maxClimbTime = 1.5f;
    [Export(PropertyHint.Range, "1.0,20.0")] private float _climbSpeed = 5f;
    [Export(PropertyHint.Range, "  0,1000")] private ulong _superGlideWindow = 50;
    [Export(PropertyHint.Range, "0.0,10.0")] private float _superGlideYStrength = 4f;
    [Export(PropertyHint.Range, "0.0,10.0")] private float _superGlideXStrength = 8f;

    private bool _isClimbing = false;
    public bool IsClimbing => _isClimbing;

    private ulong _startTime = 0;
    private Vector3 _force = Vector3.Zero;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;

    public override void _Ready() => SetPhysicsProcess(false);
    public override void _PhysicsProcess(double delta) => Climb();

    public Vector3 LedgeClimb(Vector3 velocity)
    {
        if (_isClimbing)        // Shouldn't happen ? to verify - might not neeed is climbing anymore
            return velocity;

        if (!_chestCast.IsColliding() || _headCast.IsColliding())
            return _jump.Jump(velocity); // Propagate to _jump

        if (_jumpInput.UseBuffer())
            DoLedgeClimb();             // Do it !

        return velocity;
    }

    private void DoLedgeClimb()
    {
        _startTime = Time.GetTicksMsec();
        _prevVelocity = _controller.Velocity;
        _dash.AbortDash();

        _direction = -_chestCast.GetCollisionNormal();
        _force = new Vector3(_direction.X*1.5f, _climbSpeed, _direction.Z*1.5f);
        
        _controller.TakeOverForces.AddPersistent(_force);
        _isClimbing = true;

        OnStart?.Invoke(this, EventArgs.Empty);

        SetPhysicsProcess(true);
    }


    private void Climb()
    {
        float timeElapsed = (Time.GetTicksMsec() - _startTime)/1000f;
        if (timeElapsed > _maxClimbTime || !_footCast.IsColliding())
            StopClimb();
    }

    private void StopClimb()
    {
        _startTime = 0;
        _controller.TakeOverForces.RemovePersistent(_force);

        Vector3 minOut = new(_direction.X, 1f, _direction.Z);

        Vector3 outVelocity = _prevVelocity;
        outVelocity = outVelocity.Max(minOut.Abs());
        outVelocity *= minOut.Sign();
        
        if (Time.GetTicksMsec() - _crouchInput.LastCrouchDown < _superGlideWindow)
        {
            outVelocity += _direction * _superGlideXStrength;
            outVelocity.Y = _superGlideYStrength;
        }

        _controller.Velocity = outVelocity;
        _controller.RealVelocity = outVelocity;
        SetPhysicsProcess(false);
        _isClimbing = false;
    }
}