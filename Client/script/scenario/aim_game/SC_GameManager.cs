using System;
using Client.Api;
using Client.Api.Auth;
using Client.Api.Godot;
using Client.Api.Score;
using Godot;
using Shared.Scores;

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

	public Action<Guid, int> ScoreSubmitted;

	private bool _active;

	public override void _Ready()
	{
		_statsInput.DisableAction();
	}

	public void Init(GE_IActiveCombatEntity player)
	{
		if (_active)
			return;

		if (player == null)
			return;

		_active = true;

		if (_player != player)
			InitNewPlayer(player);
		else
			GameStats.Reset();

		Score.Value = 0;

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
		
		EmitSignal(SignalName.ResetGame);

		if (save && Session.IsAuthenticated)
			SendScore(GameStats.ToScoreReq());

		
		_active = false;
	}

	private async void SendScore(SubmitScoreRequest scoreReq)
	{
		ScoreResult result = await ApiGodotGlue.Instance.SubmitScore(scoreReq);

		if (result.Success && result.ScoreId != null && result.Rank != null)
			ScoreSubmitted?.Invoke((Guid)result.ScoreId, (int)result.Rank);
	}
}
