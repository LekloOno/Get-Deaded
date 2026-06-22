using System;
using Godot;

public partial class E_FreezerMover : Node, E_IEnemyComponent
{
    public E_IEnemy? Enemy {get; set;}
    private NodePath _enemyPath = null!;
    [Export] public NodePath EnemyPath
    {
        get => _enemyPath;
        set => this.SetEnemy(this, ref _enemyPath, value);
    }

    [Export] private DATA_FreezerMover  _data = null!;
    [Export] private CharacterBody3D    _body = null!;
    [Export] private RayCast3D          _groundCast = null!;

    public override void _Ready()
    {
        _body.MotionMode = CharacterBody3D.MotionModeEnum.Floating;
        (this as E_IEnemyComponent).ResolveEnemy(this);
    }

    private double _elapsedStraffe = 0f;
    private double _elapsedFloat = 0f;
    private bool _left;
    private bool _up;
    private bool _following;
    public override void _PhysicsProcess(double delta)
    {
        if (!_dead)
        {
            SetStraffe(delta);
            SetFloat(delta);
        }

        Vector3 velocity = _body.GetRealVelocity();
        velocity.X /= 1f + _data.Drag;
        velocity.Z /= 1f + _data.Drag;

        _body.Velocity = GetVelocity(velocity, delta);
        _body.MoveAndSlide();
    }

    private Vector3 GetVelocity(Vector3 velocity, double delta)
    {
        if (_dead)
            return DeadMovement(velocity, delta);

        if (Enemy is null ||
            Enemy.Target is null)
            return CombatMovement(velocity);

        if (_following)
            return ApproachMovement(velocity, Enemy.Target);
        
        if (IsCloseEnough(Enemy.Target))
            return CombatMovement(velocity);
        
        _following = true;
        return ApproachAccelerate(velocity, Enemy.Target);
    }

    private Vector3 ApproachMovement(Vector3 velocity, GE_ICombatEntity target)
    {
        if (!FinishedApproach(target))
            return ApproachAccelerate(velocity, target);
            
        _following = false;
        return CombatMovement(velocity);
    }

    private Vector3 CombatMovement(Vector3 velocity) =>
        HorizontalAccelerate(velocity) +
        VerticalAccelerate(velocity);

    private bool IsCloseEnough(GE_ICombatEntity target) =>
        _body.GlobalPosition.DistanceTo(target.Body.GlobalTransform.Origin) < _data.FocusMaxDistance;

    private bool FinishedApproach(GE_ICombatEntity target) =>
        _body.GlobalPosition.DistanceTo(target.Body.GlobalTransform.Origin) < _data.FocusMinDistance;

    private readonly Random _rng = new();
    private void SetStraffe(double delta)
    {
        _elapsedStraffe -= delta;
        if (_elapsedStraffe > 0f)
            return;

        _left ^= true;

        float seed = _rng.NextSingle();
		_elapsedStraffe = Mathf.Lerp(_data.MinStraffe, _data.MaxStraffe, seed);
    }

    private void SetFloat(double delta)
    {
        _elapsedFloat -= delta;

        if (!GoingTooHigh() && _elapsedFloat > 0f)
            return;

        _up ^= true;

        float seed = _rng.NextSingle();
		_elapsedFloat = Mathf.Lerp(_data.MinFloat, _data.MaxFloat, seed);
    }

    private bool GoingTooHigh() =>
        _up && GetHeight() >= Mathf.Abs(_groundCast.TargetPosition.Y);

    private float GetHeight()
    {
        if (!_groundCast.IsColliding())
            return Mathf.Abs(_groundCast.TargetPosition.Y);

        Vector3 pos = _groundCast.GetCollisionPoint();
        return _groundCast.GlobalPosition.DistanceTo(pos);
    }

    private Vector3 GetStraffeDir() => _left ? _body.GlobalTransform.Basis.X : -_body.GlobalTransform.Basis.X;
    private Vector3 GetFloatDir()   => _up   ? Vector3.Up   : Vector3.Down;

    private Vector3 ApproachAccelerate(Vector3 velocity, GE_ICombatEntity target)
    {
        //velocity.Y = 0;
        Vector3 dir = target.Body.GlobalTransform.Origin - _body.GlobalPosition;
        float dirDst = dir.Length() - _data.FocusMinDistance;
        dir = dir.Normalized() * dirDst;
        dir += GetStraffeDir() * _data.Acceleration;
        dir += GetFloatDir() * _data.FloatAcceleration;

        velocity += dir * _data.FollowAcceleration;

        float speed = Mathf.Min(velocity.Length(), _data.FollowMaxSpeed);
        return velocity.Normalized() * speed;
    }

    private Vector3 HorizontalAccelerate(Vector3 velocity)
    {
        velocity.Y = 0;

        Vector3 acceleration = GetStraffeDir() * _data.Acceleration;
        velocity += acceleration;

        float speed = Mathf.Min(velocity.Length(), _data.MaxSpeed);
        return velocity.Normalized() * speed;
    }

    private Vector3 VerticalAccelerate(Vector3 velocity)
    {
        velocity.X = velocity.Z = 0;

        Vector3 acceleration = GetFloatDir() * _data.FloatAcceleration;
        velocity += acceleration;

        float speed = Mathf.Min(Mathf.Abs(velocity.Y), _data.FloatMaxSpeed);
        return velocity.Normalized() * speed;
    }

    private bool _dead;
    public void OnDied(E_IEnemy enemy, GC_Health senderLayer) =>
        _dead = true;

    private Vector3 DeadMovement(Vector3 velocity, double delta)
    {
        if (_angularVelocity.LengthSquared() > 0.0001f)
        {
            float angle = _angularVelocity.Length() * (float)delta;
            Vector3 axis = _angularVelocity.Normalized();

            _body.GlobalBasis =
                new Basis(axis, angle) * _body.GlobalBasis;

            _angularVelocity *= 0.99f;
        }

        return velocity + _body.GetGravity() * (float) delta;
    }

    public void OnPooled(E_IEnemy enemy) {}

    public void OnSpawned()
    {
        _body.GlobalBasis = Basis.Identity;
        SetPhysicsProcess(true);
        _dead = false;
    }

    public void OnDisabled(E_IEnemy enemy) => SetPhysicsProcess(false);

    public void OnEnemyChanged(E_IEnemy? prev, E_IEnemy? next)
    {
        if (prev != null)
            prev.HealthManager.HitReceived -= OnHitReceived;

        if (next != null)
            next.HealthManager.HitReceived += OnHitReceived;
    }

    private Vector3 _angularVelocity = Vector3.Zero;
    //[Export] private CharacterBody3D    _body = null!;
    private void OnHitReceived(Vector3 from, Vector3 to, HitEventArgs args)
    {
        // add some torque to the body, according to the from - to direction
        // That is, for example, if such direction points at the center of the body, it should not add any torque, but if it is parallel to the body center, it should add a lot

        Vector3 forceDirection = (to - from).Normalized();
        Vector3 center = _body.GlobalPosition;
        Vector3 r = to - center;
        Vector3 torque = r.Cross(forceDirection);

        float torqueStrength = 100.0f;

        _angularVelocity = torque * torqueStrength;
    }
}