using System;
using Client.Api;
using Client.Api.Auth;
using Client.Api.Godot;
using Godot;

[GlobalClass]
public partial class SC_GameManager : Node
{
	[Export] private SC_SpawnerScript _initial;
	[Export] public float CountDown {get; private set;} = 2f;
	[Export] private float _killRegen = 10f;
	[Signal] public delegate void InitializeEventHandler(SC_GameManager manager);
	[Signal] public delegate void InitializedEventHandler();
	[Signal] public delegate void StartGameEventHandler();
	[Signal] public delegate void ResetGameEventHandler();
	[Signal] public delegate void EarnScoreEventHandler(uint earned, uint score);
	[Export] private UIW_CombatStats _uiCombatStats;
	[Export] private PI_Stats _statsInput;
	public STAT_Combat CombatStats => GameStats.CombatStat;
	public STAT_ArenaGame GameStats {get; private set;}
	public Observable<uint> Score {get; private set;} = new();
	public SceneTreeTimer CountDownTimer;
	private GE_IActiveCombatEntity _player;
	private GC_Shield _shield;
	/// <summary>
	/// An "unexpected" stop, like the player manually stopping, or dying.
	/// </summary>
	public Action Interrupt;

	public override void _Ready()
	{
		_statsInput.DisableAction();
	}

	public void Init(GE_IActiveCombatEntity player)
	{
		if (player == null)
			return;

		if (_player != player)
			InitNewPlayer(player);
		else
			GameStats.Reset();

		foreach (PW_Weapon weapon in player.WeaponsHandler.Weapons)
				foreach (PW_Fire fire in weapon.Fires)
					fire.Ammos.Initialize();

		Score.Value = 0;

		player.WeaponsHandler.DisableFire();

		EmitSignal(SignalName.Initialize, this);
		EmitSignal(SignalName.Initialized);
		Input.MouseMode = Input.MouseModeEnum.Captured;

		CountDownTimer = GetTree().CreateTimer(CountDown, false, true);
		CountDownTimer.Timeout += Start;
	}

	private void InitNewPlayer(GE_IActiveCombatEntity player)
	{
		if (_player != null)
			_player.HealthManager.OnDie -= SendInterrupt;

		_uiCombatStats.Clear();
		GameStats = new(this, new(player));
		_uiCombatStats.AddStat(GameStats.CombatStat, Score);
					
		player.HealthManager.OnDie += SendInterrupt;
		_player = player;
	}

	private void SendInterrupt(GC_Health _)
	{
		DoInterrupt();
	}

	private void DoInterrupt(bool surrender = false)
	{   
		// We (try to) save if --
		bool save = !surrender && // the player did not surrender
					(CountDownTimer == null ||
					CountDownTimer.TimeLeft == 0); // the game and did start

		if (CountDownTimer != null)
		{
			CountDownTimer.Timeout -= Start;
			CountDownTimer = null;
		}
		
		Interrupt?.Invoke();

		Reset(save);
	}

	public void Surrender() =>
		DoInterrupt(true);

	private void Start()
	{
		CountDownTimer.Timeout -= Start;
		CountDownTimer = null;

		EmitSignal(SignalName.StartGame);

		_statsInput.EnableAction();
		
		if (_player.HealthManager.TopHealthLayer is GC_Shield shield)
			_shield = shield;

		_player.WeaponsHandler.EnableFire();
		_initial.Start(_player);
		SC_EntitiesManager.EnablePickups();
	}

	public void HandleKill(E_IEnemy enemy, GC_Health senderLayer)
	{
		Score.Value += enemy.Score;
		EmitSignal(SignalName.EarnScore, enemy.Score, Score.Value);

		if (_shield != null)
			_shield.Regen(_killRegen);
	}

	public void EndGame()
	{
		_player.HealthManager.Heal(99999);
		Reset();
	}

	private void Reset(bool save = true)
	{
		_player.WeaponsHandler.ClearDamageMultiplier();
		_statsInput.DisableAction();
		SC_EntitiesManager.DisablePickups();
		
		if (save && Session.IsAuthenticated)
			SendScore();

		EmitSignal(SignalName.ResetGame);
	}

	private async void SendScore()
	{
		await ApiGodotGlue.Instance.SubmitScore(GameStats.ToScoreReq());
	}
}
