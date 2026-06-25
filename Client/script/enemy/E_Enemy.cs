using System;
using System.Threading.Tasks;
using Godot;

public partial class E_Enemy : GB_CharacterBody, E_IEnemy
{
	private static ulong Count;
	public readonly ulong Id = Count ++;

	[Export] private E_EnemyMaterial _mat;
	[Export] private GC_HealthManager _healthManager;
	[Export] public E_EnemySettings Settings;
	[Export] private float _hideDelay;
	[Export] private AnimationTree _animationTree;
	[Export] private GL_Dropper _lootDropper;
	[Export] private float _drag = 10f;
	[Export] private Color _hitColor = new(1f, 1f, 1f, 1f);
	[Export] private float _hitTime = 0.15f;
	public uint Score => Settings.Score;
	[Export] private PhysicalBoneSimulator3D _ragdolSimulator;
	[Export] private Skeleton3D _skeleton;
	[Export] public PROTO_Mover Mover;
	[Export] public E_MoverWrapper MoverWrapper;
	[Export] private bool _aim;
	[Export] public PCT_SimpleTraumaData KillTraumaData {get; private set;}
	[Export] public PW_FireBis Fire;
	[Export] public Node3D AimPosition {get; private set;}
	[Export] public float SpeedSpreadFactor = 10f;
	[Export] public double ReactionTime = 0.2f;


	public bool Alive {get; private set;} = false;
	private bool _inPool = true;
	private SceneTreeTimer _hideTimer;
	public event EnemyHealthEventHandler? Died;
	private void PropagDie(GC_Health sender) =>
		Died?.Invoke(this, sender);

	public event EnemyDisableEventHandler? Disabled;
	public event EnemyHealthEventHandler<DamageEventArgs> Damaged;
	private void PropagDamage(GC_Health sender, DamageEventArgs damage) =>
		Damaged?.Invoke(this, sender, damage);

	private BaseMaterial3D.ShadingModeEnum _initialJointShadingMode;
	private Color _initialColor;
	private Color _initialJointColor;
	private SceneTreeTimer _hitResetTimer;
	private GE_ICombatEntity? _target;
	private Node3D _targetNode;

	public event Action Spawned;
	public event Action<E_IEnemy>? Pooled;

	public void SetTarget(Node3D target) => Mover.Target = target;

	public GC_HealthManager HealthManager => _healthManager;

	public PW_WeaponsHandler WeaponsHandler => null;

	public GB_IExternalBodyManager Body => this;

	public GE_ICombatEntity? Target
	{
		get => _target;
		set {
			if (value is Node3D node)
				Mover.Target = node;
			_target = value;
		}
	}

	public override void _Ready()
	{
		UpdateSettings();
		SetProcess(false);

		Spawn();

		Died += PlayDeath;
	}

	public void SetSettings(E_EnemySettings settings)
	{
		if (Settings != null)
			Settings.Updated -= UpdateSettings;

		Settings = settings;
		Settings.Updated += UpdateSettings;
		UpdateSettings();		
	}

	private void UpdateSettings()
	{
		PW_FireBis fire = Settings.Fire.Instantiate<PW_FireBis>();
		Fire?.QueueFree();
		Fire = fire;
		AimPosition.AddChild(Fire);
		Fire?.Initialize(this);
		
		GC_Health healthTree = Settings.Health.BuildNode();
		if (HealthManager.TopHealthLayer != null)
		{
			HealthManager.TopHealthLayer.QueueFree();
			HealthManager.TopHealthLayer.OnDie -= PropagDie;
			HealthManager.TopHealthLayer.OnDamage -= PropagDamage;
		}
		HealthManager.TopHealthLayer = healthTree;
		HealthManager.TopHealthLayer.OnDie += PropagDie;
		HealthManager.TopHealthLayer.OnDamage += PropagDamage;
		HealthManager.AddChild(healthTree);


		if (Mover == null)
		{
			PROTO_Mover mover = new(Settings.MoverData, this);
			MoverWrapper.Mover = mover;
			Mover = mover;
			AddChild(mover);
		}
		else
			Mover.Data = Settings.MoverData;
	}

	public void PlayDeath(E_IEnemy _, GC_Health health)
	{
		_lootDropper.Drop();
		_ragdolSimulator?.PhysicalBonesStartSimulation();
		DeathDisable();
	}

	public void DeathDisable()
	{
		DisableActions();
		if (_mat.AnimatingDisable)
			return;
		
		_mat.DisableCompleted += OnDisableCompleted;
		_mat.SmoothDisable();
	}

	private void OnDisableCompleted()
	{
		_mat.DisableCompleted -= OnDisableCompleted;
		DisableBase();
	}

	private void DisableBase()
	{
		Hide();
		SetPhysicsProcess(false);
		_animationTree.Active = false;
		_ragdolSimulator.PhysicalBonesStopSimulation();
		_ragdolSimulator.Active = false;
		_ragdolSimulator.ProcessMode = ProcessModeEnum.Disabled;
		_skeleton.ProcessMode = ProcessModeEnum.Disabled;
		ProcessMode = ProcessModeEnum.Disabled;
		Disabled?.Invoke(this);
	}

	private void DisableActions()
	{
		Fire?.Disable();

		if (!Alive)
			return;

		SetProcess(false);
		
		Velocity = Vector3.Zero;

		Alive = false;
		CollisionLayer = 0;
		_healthManager.DisableHurt();
	}

	public void Pool()
	{
		if (_inPool)
			return;

		if (Alive)
			DisableActions();

		_inPool = true;
		DisableBase();
		Pooled?.Invoke(this);
		//_hideTimer = GetTree().CreateTimer(_hideDelay);
		//_hideTimer.Timeout += Hide;
	}

	public void Spawn()
	{
		if (!_inPool)
			return;

		if (Alive)
			return;

		Fire?.Enable();

		if (Mover != null)
			SetProcess(true);

		Alive = true;
		_inPool = false;
		CollisionLayer = CONF_Collision.Layers.EnvironmentEntity;

		//_mat.Oui();
		Show();
		SetPhysicsProcess(true);
		ProcessMode = ProcessModeEnum.Inherit;
		_skeleton.ProcessMode = ProcessModeEnum.Inherit;
		_ragdolSimulator.Active = true;
		_animationTree.Active = true;
		_ragdolSimulator.ProcessMode = ProcessModeEnum.Inherit;
		_healthManager.EnableHurt(CONF_Collision.Layers.EnnemiesHurtBox);
		_ragdolSimulator?.PhysicalBonesStopSimulation();
		_skeleton?.ResetBonePoses();
	
		_healthManager.Init(true);

		Spawned?.Invoke();
	}

	protected override void PhysicsProcessSpec(double delta)
	{
		if (!Alive)
			return;

		Vector3 velocity = Velocity;
		if (!IsOnFloor())
			velocity += GetGravity() * (float) delta;
		else
		{
			velocity = ApplyDrag(velocity, delta);
			
			if (Mover != null)
				velocity += Mover.GetAcceleration(velocity, delta);
		}
		
		Velocity = velocity;
		
		MoveAndSlide();
	}

	public Vector3 ApplyDrag(Vector3 velocity, double deltaTime)
	{
		float dragFactor = 1f/(1f+(float)deltaTime*_drag);    // Transform the drag to a velocity coeficient
		return velocity * dragFactor;
	}
}
