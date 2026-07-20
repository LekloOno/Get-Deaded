using Godot;
using System;

[GlobalClass]
public partial class PROTO_Mover : Node
{
	public PROTO_Mover() {}
	public PROTO_Mover(PROTO_MoverData data, Node3D dirNode) : this()
	{
		Data = data;
		_dirNode = dirNode;
	}

	[Export] public PROTO_MoverData Data;
	[Export] private Node3D _dirNode;
	public Node3D Target;

	private Vector2 _walkAxis = Vector2.Zero;
	public Vector2 WalkAxis {get => _walkAxis;}
	public Vector3 WishDir => ComputeWishDir();
	private Timer _straffeTimer;
	private Timer _speedTimer;
	private Random _rng = new Random();
	private float _speed = 1f;
	private bool _walking;

	public override void _Ready()
	{
		_walkAxis.X = _rng.Next(2) * 2f - 1f;
		_straffeTimer = new()
		{
			ProcessMode = ProcessModeEnum.Pausable,
			ProcessCallback = Timer.TimerProcessCallback.Physics
		};
		_straffeTimer.Timeout += ChangeStraffeDir;
		AddChild(_straffeTimer);
		StartStraffeTimer();

		_speedTimer = new()
		{
			ProcessMode = ProcessModeEnum.Pausable,
			ProcessCallback = Timer.TimerProcessCallback.Physics
		};
		_speedTimer.Timeout += ChangeSpeed;
		AddChild(_speedTimer);
		StartSpeedTimer();
	}

	private Vector3 ComputeWishDir() => _dirNode.Transform.Basis.Z * WalkAxis.Y
										+ _dirNode.Transform.Basis.X * WalkAxis.X;

	public void Rotate(Node3D self)
	{
		if (Target == null)
			return;

		Vector3 target = Target.GlobalPosition;
		target.Y = self.GlobalPosition.Y;

		self.LookAt(target);
	}

	public void ChangeStraffeDir()
	{
		_walkAxis.X *= -1;

		float seed = _rng.NextSingle();
		if (seed < Data.StraightProbability)
		{
			seed -= Data.StraightProbability / 2;
			_walkAxis.Y = Math.Sign(seed);
		}
		else
			_walkAxis.Y = 0;

		_walkAxis = _walkAxis.Normalized();
		
		StartStraffeTimer();
	}

	private void StartStraffeTimer()
	{
		float seed = _rng.NextSingle();
		float nextStaffe = Mathf.Lerp(Data.MinStraffe, Data.MaxStraffe, seed); 
		_straffeTimer.Start(nextStaffe);
	}

	public void ChangeSpeed()
	{
		float seed = _rng.NextSingle();
		_speed = PickSpeed(seed);
		StartSpeedTimer();
	}

	public float PickSpeed(float seed)
	{
		if (seed < Data.PropWalk)
			return Data.WalkSpeed;
		
		seed -= Data.PropWalk;
		
		if (seed < Data.PropRun)
			return Data.RunSpeed;

		return Data.SprintSpeed;
	}

	public void StartSpeedTimer()
	{
		float seed = _rng.NextSingle();
		float nextChange = Mathf.Lerp(Data.MinHold, Data.MaxHold, seed);
		_speedTimer.Start(nextChange);
	}
	

	public bool GetAcceleration(Vector3 velocity, double delta, out Vector3 accel)
	{
		accel = PHX_MovementPhysics.Acceleration(_speed, Data.Acceleration, velocity, WishDir.Normalized(), (float)delta);
		return (velocity + accel).Length() > _speed;
	}
}
