using Godot;

public partial class E_EnemyAimer : Node
{
    [Export] private E_Enemy _owner = null!;

    private bool _shooting;
    private double _timeOnSight = 0f;
    private bool _onSight;
	private bool _processTick;

    public override void _Ready()
    {
        SetPhysicsProcess(false);

        _owner.Died += OnDied;
        _owner.Disabled += OnDisabled;
        _owner.Spawned += Enable;
    }

    private void OnDisabled(E_IEnemy enemy) => Disable();
    private void OnDied(E_IEnemy enemy, GC_Health senderLayer) => Disable();

    public bool Enabled {get; private set;}

    public void Disable()
    {
        if (!Enabled)
            return;

        Enabled = false;
		_timeOnSight = 0;
    	_onSight = false;
        SetPhysicsProcess(false);
		_owner.Fire.Disable();
		_shooting = false;
    }

    public void Enable()
    {
        if (Enabled)
            return;

        Enabled = true;
        SetPhysicsProcess(true);

        _owner.Fire.Enable();
    }

	private bool _targetBehind;
	public void Attack(E_Enemy owner, double delta)
	{
		if (owner.Fire == null || owner.Target == null)
			return;

		ulong tick = Engine.GetPhysicsFrames();

		_processTick = (tick & 7) == (owner.Id & 7);
		if (_processTick)
		{
			_targetBehind = DetectBehind(owner);
			_onSight = DetectSight(owner);
		}

		if (!_onSight)
			_timeOnSight = 0;
		else if (!_targetBehind)
			_timeOnSight += delta;

		bool nextShoot = !_targetBehind &&
			_timeOnSight >= owner.Settings.ReactionTime;

		if (nextShoot)
			Aim(owner);

		if (_shooting == nextShoot)
			return;

		if (nextShoot)
			_shooting = owner.Fire.Press();
		else
			_shooting = !owner.Fire.Release();
	}

	private bool DetectBehind(E_Enemy owner)
	{
		Vector3 targetPos = owner.Target.Body.GlobalTransform.Origin;
		Vector3 dir = targetPos - owner.GlobalPosition;
        dir.Y = 0;

        if (dir.LengthSquared() < 0.0001f)
            return false;

        Vector3 selfDir = - owner.GlobalTransform.Basis.Z;
		float hitAngle = MATH_Vector3Ext.FlatAngle(selfDir, dir);
        return Mathf.RadToDeg(hitAngle) > 120;
	}

	private bool DetectSight(E_Enemy owner)
	{
		Vector3 from = owner.Fire.GlobalPosition;
		Vector3 to = owner.Target.Body.GlobalTransform.Origin;

		var spaceState = owner.GetWorld3D().DirectSpaceState;

		var query = PhysicsRayQueryParameters3D.Create(from, to);

		query.CollisionMask = 1;
		var result = spaceState.IntersectRay(query);

		return result.Count == 0;
	}

	private void Aim(E_Enemy owner)
	{
		float spread = SpreadFromTarget(owner) * owner.Settings.SpreadFactor;
		LookAtWithSpread(owner.Fire, owner.Target.Body.GlobalTransform.Origin, spread);
	}

	private float _cachedSpread = 0f;
	public float SpreadFromTarget(E_Enemy owner)
	{
		if (!_processTick)
			return _cachedSpread;

		Vector3 delta = owner.GlobalPosition - owner.Target.Body.GlobalTransform.Origin;
		Vector3 direction = delta.Normalized();
		Vector3 targetVel = owner.Target.Body.Velocity();

		float lateralSpeed = (targetVel - targetVel.Dot(direction) * direction).Length();
		_cachedSpread = lateralSpeed / delta.Length();
		return _cachedSpread;
	}

	private static RandomNumberGenerator _rng = new();

	public static void LookAtWithSpread(
		Node3D node,
		Vector3 targetPosition,
		float spreadDegrees,
		Vector3? up = null)
	{
		Vector3 upVector = up ?? Vector3.Up;
		Vector3 direction = (targetPosition - node.GlobalPosition).Normalized();
		Vector3 spreadDirection = ApplySpread(direction, spreadDegrees);
		Vector3 lookPoint = node.GlobalPosition + spreadDirection;

		node.LookAt(lookPoint, upVector);
	}

	private static Vector3 ApplySpread(Vector3 direction, float spreadDegrees)
	{
		if (spreadDegrees <= 0f)
			return direction;

		Vector3 randomAxis = direction.Cross(
			new Vector3(
				_rng.RandfRange(-1f, 1f),
				_rng.RandfRange(-1f, 1f),
				_rng.RandfRange(-1f, 1f)
			).Normalized()
		).Normalized();

		float angle = Mathf.DegToRad(
			_rng.RandfRange(-spreadDegrees, spreadDegrees)
		);

		return direction.Rotated(randomAxis, angle).Normalized();
	}

    public override void _PhysicsProcess(double delta)
    {
        Attack(_owner, delta);
    }
}