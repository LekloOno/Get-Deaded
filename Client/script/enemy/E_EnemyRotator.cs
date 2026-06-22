using Godot;

public partial class E_EnemyRotator : Node, E_IEnemyComponent
{
    public E_IEnemy? Enemy {get; set;}
    private NodePath _enemyPath = null!;
    [Export] public NodePath EnemyPath
    {
        get => _enemyPath;
        set => this.SetEnemy(this, ref _enemyPath, value);
    }
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

    [Export] public bool _pitch;
    [Export] public Node3D? _pitchNode;

    private void UpdateTurnSpeed() => _turnSpeedRad = Mathf.DegToRad(_turnSpeed);

	private float _turnSpeed = 400f;
	private float _turnSpeedRad;

	[Export] private double _turnAroundDelay = 0.2f;
    private double _turnAroundDelayAcc;

    public override void _Ready()
    {
        (this as E_IEnemyComponent).ResolveEnemy(this);

        UpdateTurnSpeed();
        if (Enemy == null)
            return;
    }

    public void OnDisabled(E_IEnemy enemy) => Disable();
    public void OnDied(E_IEnemy enemy, GC_Health senderLayer) => Disable();
    public void OnPooled(E_IEnemy enemy) {}
    public void OnSpawned() => Enable();
    public void OnEnemyChanged(E_IEnemy? prev, E_IEnemy? next) {}

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
	public void LookAtTarget(E_IEnemy owner, Vector3 target, double delta)
	{
        Vector3 dir = target - owner.Body.GlobalTransform.Origin;
        Vector3 yawDir = dir;
        yawDir.Y = 0;

        if (dir.LengthSquared() < 0.0001f)
            return;

        Vector3 selfDir = - owner.Body.GlobalTransform.Basis.Z;
		float hitAngle = MATH_Vector3Ext.FlatAngle(selfDir, yawDir);
        
        yawDir = -yawDir.Normalized();
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


        owner.Body.SetRotation(new Vector3(
            owner.Body.Rotation.X,
            Mathf.LerpAngle(owner.Body.Rotation.Y, targetYaw, _turnSpeedRad * (float) delta),
            owner.Body.Rotation.Z
        ));

        if (!_pitch || _pitchNode is null)
            return;

        Vector3 pitchDir = dir.Normalized();
        float targetPitch = Mathf.Asin(pitchDir.Y);

        _pitchNode.Rotation = new Vector3(
            Mathf.LerpAngle(_pitchNode.Rotation.X, targetPitch, _turnSpeedRad * (float) delta),
            _pitchNode.Rotation.Y,
            _pitchNode.Rotation.Z
        );
	}

    public override void _PhysicsProcess(double delta)
    {
        if (Enemy == null)
            return;

        if (Enemy.Target == null)
            return;

        LookAtTarget(Enemy, Enemy.Target.Body.GlobalTransform.Origin, delta);
    }
}