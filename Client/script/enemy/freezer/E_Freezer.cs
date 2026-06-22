using System;
using System.Threading.Tasks;
using Godot;

public partial class E_Freezer : Node3D, E_IEnemy
{
	[Export] public  E_FreezerSettings          Settings = null!;
    [Export] private E_EnemyMaterial            _mat = null!;
    [Export] private GB_CharacterBodyManager    _body = null!;
    [Export] public  PCT_SimpleTraumaData       KillTraumaData  { get; private set; } = null!;
	[Export] public  GC_HealthManager           HealthManager   { get; private set; } = null!;
    public uint Score => Settings.Score;

    public GE_ICombatEntity Target { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public PW_WeaponsHandler WeaponsHandler => null!;
    public GB_IExternalBodyManager Body => _body;

    public event EnemyHealthEventHandler? Died;
    private void PropagDie(GC_Health sender) =>
		Died?.Invoke(this, sender);
    public event EnemyDisableEventHandler? Disabled;
    public event EnemyHealthEventHandler<DamageEventArgs>? Damaged;
    private void PropagDamage(GC_Health sender, DamageEventArgs damage) =>
		Damaged?.Invoke(this, sender, damage);
    public event Action<E_IEnemy>? Pooled;
    public event Action? Spawned;

    public bool Enabled {get; private set;} = false;
    public override void _Ready()
	{
		UpdateSettings();
		SetProcess(false);

		Spawn();

		Died += PlayDeath;
	}

    public void Pool()
    {
        DisableActions();
		DisableBase();
		Pooled?.Invoke(this);
    }

    public void Spawn()
    {
        if (Enabled)
			return;

		Enabled = true;
		_body.CharacterBody.CollisionLayer = CONF_Collision.Layers.EnvironmentEntity;

		Show();
		SetPhysicsProcess(true);
		ProcessMode = ProcessModeEnum.Inherit;
		HealthManager.EnableHurt(CONF_Collision.Layers.EnnemiesHurtBox);
		HealthManager.Init(true);

		Spawned?.Invoke();
    }

    private void UpdateSettings()
	{
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
	}

    public void SetSettings(E_FreezerSettings settings)
	{
		if (Settings != null)
			Settings.Updated -= UpdateSettings;

		Settings = settings;
		Settings.Updated += UpdateSettings;
		UpdateSettings();		
	}

    private void DisableActions()
	{
		if (!Enabled)
			return;

		SetProcess(false);
		
		_body.Body.ResetVelocity(Vector3.Zero);

		Enabled = false;
		_body.CharacterBody.CollisionLayer = 0;
		HealthManager.DisableHurt();
	}

    private void DisableBase()
	{
		Hide();
		SetPhysicsProcess(false);
		ProcessMode = ProcessModeEnum.Disabled;
	}

    public void PlayDeath(E_IEnemy _, GC_Health health) => DeathDisableAsync();

    public async Task DeathDisableAsync()
	{
		DisableActions();
		await _mat.SmoothDisable();
		DisableBase();
		Disabled?.Invoke(this);
	}
}