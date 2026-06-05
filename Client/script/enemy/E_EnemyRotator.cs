using Godot;

public partial class E_EnemyRotator : Node
{
    [Export] private E_Enemy? _owner;
    [Export] private float TurnSpeed
	{
		get => _turnSpeed;
		set
		{
			if (value == _turnSpeed)
				return;

			_turnSpeed = value;
			UpdateTurnSpeed();
		}
	}

	private void UpdateTurnSpeed() => _turnSpeedRad = Mathf.DegToRad(_turnSpeed);

	private float _turnSpeed = 400f;
	private float _turnSpeedRad;

	[Export] private double _turnAroundDelay = 0.2f;
    private double _turnAroundDelayAcc;

    public override void _Ready()
    {
        UpdateTurnSpeed();
        if (_owner == null)
            return;

        _owner.Died += OnDied;
        _owner.Disabled += OnDisabled;
        _owner.Spawned += Enable;
    }

    private void OnDisabled(E_IEnemy enemy) => Disable();
    private void OnDied(E_IEnemy enemy, GC_Health senderLayer) => Disable();

    private void Disable()
    {
        SetPhysicsProcess(false);
    }

    private void Enable()
    {
        SetPhysicsProcess(true);
    }

    private bool _turningAround;
    private bool _isBehind;
	public void LookAtTarget(Node3D owner, Vector3 target, double delta)
	{
        Vector3 dir = target - owner.GlobalPosition;
        dir.Y = 0;

        if (dir.LengthSquared() < 0.0001f)
            return;

        Vector3 selfDir = - owner.GlobalTransform.Basis.Z;
		float hitAngle = MATH_Vector3Ext.FlatAngle(selfDir, dir);
        
        Vector3 yawDir = -dir.Normalized();
        float targetYaw = Mathf.Atan2(yawDir.X, yawDir.Z);

		_isBehind = Mathf.RadToDeg(hitAngle) > 120;

        if (!_isBehind)
            _turningAround = false;
        else if (!_turningAround)
        {
            _turningAround = _turnAroundDelayAcc >= _turnAroundDelay;
            
            if (_turningAround)
                _turnAroundDelayAcc = 0;
            else
            {
                _turnAroundDelayAcc += delta;
                return;
            }
        }


        owner.Rotation = new Vector3(
            owner.Rotation.X,
            Mathf.LerpAngle(owner.Rotation.Y, targetYaw, _turnSpeedRad * (float) delta),
            owner.Rotation.Z
        );
	}

    public override void _PhysicsProcess(double delta)
    {
        if (_owner == null)
            return;

        if (_owner.Target == null)
            return;

        LookAtTarget(_owner, _owner.Target.Body.GlobalTransform.Origin, delta);
    }
}