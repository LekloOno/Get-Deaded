using System.Collections.Generic;
using Godot;

[GlobalClass, Tool]
public partial class JumpPad : Node3D
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
	/// <summary>
	/// The hitbox used to detect target entities. <br/>
	/// Should be set as the child of this node, it is auto-assigned at runtime.
	/// </summary>
	private Area3D _hitbox;

	public override void _Ready()
	{
		if (_hitbox != null)
			_hitbox.CollisionMask = CONF_Collision.Layers.EnvironmentEntity;
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

		bodyManager.HandleKnockBack(Transform.Basis.Z * _strength);
		EmitSignal(SignalName.Launch);
	}

	// +-------------------+
	// |      EDITOR       |
	// +-------------------+
	// _____________________
	private void SetHitbox(Area3D hitbox)
	{
		if (_hitbox != null)
			_hitbox.BodyEntered -= OnBodyEntered;

		_hitbox = hitbox;
		_hitbox.BodyEntered += OnBodyEntered;
		if (!Engine.IsEditorHint() && CONF_Collision.Instance != null)
			_hitbox.CollisionMask = CONF_Collision.Layers.EnvironmentEntity;
	}

	public override void _EnterTree()
	{
		if (RetrieveHitbox(out Area3D hitbox))
			SetHitbox(hitbox);

		UpdateConfigurationWarnings();
	}

	public override void _Notification(int what)
	{
		if (what != NotificationChildOrderChanged)
			return;
		
		if (RetrieveHitbox(out Area3D hitbox))
			SetHitbox(hitbox);

		UpdateConfigurationWarnings();
	}

	/// <summary>
	/// Tries to retrieve a hitbox node from children.
	/// </summary>
	/// <param name="hitbox">A valid Area3D node, might be null.</param>
	/// <returns>Whether a valid hitbox was found.</returns>
	private bool RetrieveHitbox(out Area3D hitbox)
	{
		foreach(Node node in GetChildren())
			if (node is Area3D area)
			{
				hitbox = area;
				return true;
			}

		hitbox = null;
		return false;
	}

	// +-------------------+
	// |  CONFIG WARNINGS  |
	// +-------------------+
	// _____________________
	public override string[] _GetConfigurationWarnings()
	{
		List<string> warnings = [];

		if (_hitbox == null)
			warnings.Add("This node has no attached hitbox.\nConsider adding an Area3D children.");

		return [.. warnings];
	}
}
