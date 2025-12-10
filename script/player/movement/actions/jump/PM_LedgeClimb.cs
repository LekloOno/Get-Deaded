using System;
using System.ComponentModel.DataAnnotations.Schema;
using Godot;

[GlobalClass]
public partial class PM_LedgeClimb : PM_Action
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0.0, 2.0")] private float _maxClimbTime = 0.5f;
    [Export(PropertyHint.Range, "1.0,20.0")] private float _climbSpeed = 4f;
    [Export(PropertyHint.Range, "  0,1000")] private ulong _superGlideWindow = 80;
    [Export(PropertyHint.Range, "0.0,10.0")] private float _superGlideYStrength = 4f;
    [Export(PropertyHint.Range, "0.0,10.0")] private float _superGlideXStrength = 8f;
    [Export] private float _minSpace = 0.3f;    // The minimum space considered as a valid platform to climb to.
    [Export] private float _minHeight = 0.4f;  // Obstacles lower than this can't be ledgeclimbed.
    
    [ExportCategory("Setup")]
    [Export] private PI_Jump _jumpInput;
    [Export] private PI_CrouchDispatcher _crouchInput;
    [Export] private PI_Walk _walkInput;
    [Export] private PM_Controller _controller;
    [Export] private PM_Dash _dash;
    [Export] private PM_Jump _jump;
    [Export] private PHX_BodyScale _bodyScale;
    [Export] private Node3D _pivot;
    
    [Export] private ShapeCast3D _ledgeCast; 
    // The direction of the cast is computed for each try. This determines the size and origin of the cast only.

    private bool _isClimbing = false;
    public bool IsClimbing => _isClimbing;

    private ulong _startTime = 0;
    private Vector3 _force = Vector3.Zero;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private KinematicCollision3D _lastCollision;

    public override void _Ready()
    {
        _ledgeCast.Enabled = false;
        SetPhysicsProcess(false);
    }
    public override void _PhysicsProcess(double delta) => Climb();

    public Vector3 LedgeClimb(Vector3 velocity)
    {
        if (_isClimbing)        // Shouldn't happen ? to verify - might not neeed is climbing anymore
            return velocity;

        if (!CheckPhysics())
            return _jump.Jump(velocity); // Propagate to _jump

        if (_jumpInput.UseBuffer())
            DoLedgeClimb();             // Do it !

        return velocity;
    }

    public Vector3 LazyLedgeClimb(Vector3 velocity)
    {
        if (!_isClimbing && (_jumpInput.UseBuffer() || _jumpInput.IsDown))
            DoLedgeClimb();             // Do it !

        return velocity;
    }

    public bool CanLedgeClimb() => CheckPhysics() && !_isClimbing;
    private bool CheckPhysics() => PHX_Checks.CanLedgeClimb(_controller, _bodyScale.Collider, _pivot, _minSpace, _minHeight, _ledgeCast, out _lastCollision);

    public void DoLedgeClimb()
    {
        _startTime = Time.GetTicksMsec();
        _prevVelocity = _controller.VelocityCache.UseCacheOr(_controller.RealVelocity);
        _dash.AbortDash();

        _direction = -_lastCollision.GetNormal();
        _force = new Vector3(_direction.X*1.5f, _climbSpeed, _direction.Z*1.5f);
        
        _controller.TakeOverForces.AddPersistent(_force);
        _isClimbing = true;

        OnStart?.Invoke(this, EventArgs.Empty);

        SetPhysicsProcess(true);
    }


    private void Climb()
    {
        float timeElapsed = (Time.GetTicksMsec() - _startTime)/1000f;
        if (timeElapsed > _maxClimbTime || PHX_Checks.CanMoveForward(_controller, _bodyScale.Collider, _pivot, 0.5f, out _lastCollision))
            StopClimb();
    }

    private void StopClimb()
    {
        _startTime = 0;
        _controller.TakeOverForces.RemovePersistent(_force);

        Vector3 minOut = new(_direction.X, 1f, _direction.Z);

        Vector3 outVelocity = _prevVelocity;
        outVelocity.Y = Mathf.Max(0f, outVelocity.Y);
        
        if(minOut.Length() > outVelocity.Length())
            outVelocity = minOut;
        
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