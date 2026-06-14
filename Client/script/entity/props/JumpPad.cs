using Godot;

[GlobalClass, Tool]
public partial class JumpPad : Area3D
{
	[Signal] public delegate void LaunchEventHandler();
	/// <summary>
	/// Strength of the impulsion. <br/>
	/// Multiplied with the direction of the Node to compute the force vector.
	/// </summary>
	[Export] private float _strength = 10f;
	/// <summary>
	/// Between 0 and 1, the proportion of the target's velocity that is preserved. <br/>
	/// - 0 means the velocity of the target will exactly be direction*_strength. <br/>
	/// - 1 means direction*_strength will be simply added to the target's current velocity.
	/// </summary>
	[Export] private float _momentum = 1f;
	[Export] public float AngleDegrees
	{
		get => _angleDegrees;
		set
		{
			if (value == _angleDegrees)
				return;

			_angleDegrees = value;
			UpdateDir();
		}
	}
	
	
	private float _angleDegrees = 30f;
	private Vector3 _dir;

	private void UpdateDir() =>
		_dir = (-Transform.Basis.Z).Rotated(Transform.Basis.X, Mathf.DegToRad(_angleDegrees));

	public override void _Ready()
	{
		CollisionMask = CONF_Collision.Layers.EnvironmentEntity;
		BodyEntered += OnBodyEntered;
		UpdateDir();
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is not GB_IExternalBodyManager bodyManager)
			return;

		if (_momentum != 1f)
		{
			Vector3 vel = bodyManager.Velocity() * (1f - _momentum);
			bodyManager.HandleKnockBack(-vel);
		}

		bodyManager.HandleKnockBack(_dir * _strength);
		EmitSignal(SignalName.Launch);
	}
}
