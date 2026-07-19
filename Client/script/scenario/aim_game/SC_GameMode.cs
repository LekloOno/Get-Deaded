using System;
using Godot;

[GlobalClass]
public abstract partial class SC_GameMode : Node, SC_IGameMode
{
	public GE_IActiveCombatEntity? Player {get; private set;}

	public event Action? Initialized;
	public event Action? Started;
	public event Action<GameModeEnd>? Interrupted;
	public event Action? Reseted;

	public event Action<E_IEnemy, GC_Health>? EnemyKilled;

	private bool _active;

	public override void _Ready()
	{
		ReadySpec();
	}

	public bool Init(GE_IActiveCombatEntity starter)
	{
		if (_active)
			return false;

		if (starter == null)
			return false;

		_active = true;

		if (Player != null)
			Player.HealthManager.OnDie -= OnPlayerDeath;

		starter.HealthManager.OnDie += OnPlayerDeath;

		InitSpec(starter);
		Player = starter;

		Initialized?.Invoke();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		return true;
	}

	public bool Interrupt(GameModeEnd outcome)
	{
		if (!_active)
			return false;

		if (!InterruptSpec(outcome))
			return false;

		Interrupted?.Invoke(outcome);
		Reset();

		return true;
	}

	private void Reset()
	{
		Player?.WeaponsHandler.ClearDamageMultiplier();
		Reseted?.Invoke();
		_active = false;
	}

	public void Surrender() => Interrupt(GameModeEnd.Surrender);
		
	public bool Start()
	{
		if (!StartSpec())
			return false;

		Started?.Invoke();
		return true;
	}

	public void HandleKill(E_IEnemy enemy, GC_Health senderLayer)
		=> EnemyKilled?.Invoke(enemy, senderLayer);

	protected abstract void ReadySpec();

	protected abstract bool StartSpec();
	protected abstract bool InitSpec(GE_IActiveCombatEntity starter);
	protected abstract bool InterruptSpec(GameModeEnd outcome);

	protected abstract void OnPlayerDeath(GC_Health senderLayer);
}
