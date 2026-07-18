using System;
using System.ComponentModel.DataAnnotations.Schema;
using Godot;

[GlobalClass]
public partial class PM_LedgeClimb : PM_Action
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0.0, 2.0")] private float _maxClimbTime = 0.5f;
    [Export(PropertyHint.Range, "1.0,20.0")] private float _climbSpeed = 4f;
    
    [ExportCategory("Physics")]
    [Export] private float _minSpace = 0.2f;    // The minimum space considered as a valid platform to climb to.
    [Export] private float _minHeight = 0.25f;  // Obstacles lower than this can't be ledgeclimbed.

    [ExportCategory("Vault")]
    [Export(PropertyHint.Range, "  0,1000")] private ulong _vaultWindow = 80;
    [Export] private Vector2 _vaultMinStrength = new(3.5f, 8f);
    [Export] private float _vaultBoost       = 4f;
    [Export(PropertyHint.Range, "0.0,1.0")] private float _vaultYmomentum = 0f;
    
    
    [ExportCategory("Setup")]
    [Export] private PI_Jump _jumpInput = null!;
    [Export] private PI_CrouchDispatcher _crouchInput = null!;
    [Export] private PI_Walk _walkInput = null!;
    [Export] private PM_Controller _controller = null!;
    [Export] private PM_Dash _dash = null!;
    [Export] private PM_Jump _jump = null!;
    [Export] private PHX_BodyScale _bodyScale = null!;
    [Export] private Node3D _pivot = null!;
    
    [Export] private ShapeCast3D _ledgeCast; 
    // The direction of the cast is computed for each try. This determines the size and origin of the cast only.

    private bool _isClimbing = false;
    public bool IsClimbing => _isClimbing;

    private ulong _startTime = 0;
    private Vector3 _force = Vector3.Zero;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private Vector3 _directionFlat = Vector3.Zero;
    private KinematicCollision3D _lastCollision;

    public event Action? SuperGlideStarted;
    public ulong LastSuperGlide { get; private set; }

    public override void _Ready()
    {
        _ledgeCast.Enabled = false;
        SetPhysicsProcess(false);
        _walkInput.BackwardPressed += OnBackwardPressed;
    }

    private void OnBackwardPressed()
    {
        if (_isClimbing)
            StopClimb();
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
    private bool CheckPhysics()
    {
        Vector3 initSize = _bodyScale.Collider.Size;
        _bodyScale.Collider.Size = new(0.3f, initSize.Y, 0.3f);

        bool canClimb = PHX_Checks.CanLedgeClimb(
            _controller, _bodyScale.Collider, _pivot,
            _minSpace, _minHeight, _ledgeCast,
            out _lastCollision);

        _bodyScale.Collider.Size = initSize;
        return canClimb;
    }

    public void DoLedgeClimb()
    {
        _startTime = PHX_Time.ScaledTicksMsec;
        _prevVelocity = _controller.VelocityCache.UseCacheOr(_controller.RealVelocity);
        _dash.AbortDash();

        _direction = -_lastCollision.GetNormal();
        
        _directionFlat = _direction;
        _directionFlat.Y = 0f;
        _directionFlat = _directionFlat.Normalized();

        _force = new Vector3(_directionFlat.X*1.5f, _climbSpeed, _directionFlat.Z*1.5f);
        
        _controller.TakeOverForces.AddPersistent(_force);
        _isClimbing = true;

        InvokeStart();

        SetPhysicsProcess(true);
    }


    private void Climb()
    {
        float timeElapsed = (PHX_Time.ScaledTicksMsec - _startTime)/1000f;
        if (timeElapsed > _maxClimbTime || PHX_Checks.CanMoveAlong(_controller, _bodyScale.Collider, _directionFlat, 0.15f, out _lastCollision))
            StopClimb();
    }

    private void StopClimb()
    {
        _startTime = 0;
        _controller.TakeOverForces.RemovePersistent(_force);

        Vector3 outVelocity = GetOutVelocity();

        _controller.Velocity = outVelocity;
        _controller.RealVelocity = outVelocity;
        _jumpInput.ResetLastJumped();
        SetPhysicsProcess(false);
        _isClimbing = false;
    }

    private Vector3 GetOutVelocity()
    {
        if (PHX_Time.ScaledTicksMsec - _crouchInput.LastCrouchDown < _vaultWindow)
        {
            Vector3 momentum = _prevVelocity;
            momentum.Y *= _vaultYmomentum;

            float speed = momentum.Length();
            speed += _vaultBoost;
            speed = Mathf.Max(_vaultMinStrength.X, speed);

            Vector3 dir = GetVaultDirection();

            Vector3 outVelocity = dir * speed;
            outVelocity.Y = _vaultMinStrength.Y;
 
            LastSuperGlide = PHX_Time.ScaledTicksMsec;
            SuperGlideStarted?.Invoke();
            return outVelocity;
        } else
            return new(_directionFlat.X, 1f, _directionFlat.Z);
    }

    public Vector3 GetVaultDirection()
    {
        if (_walkInput.WalkAxis != Vector2.Zero)
            return _walkInput.WishDir;
        
        return _walkInput.FlatDir;
    }
}