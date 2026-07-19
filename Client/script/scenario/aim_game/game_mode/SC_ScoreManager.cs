using System;
using Client.Api.Auth;
using Client.Api.Godot;
using Client.Api.Score;
using Godot;
using Shared.Scores;

public partial class SC_ScoreManager : Node, SC_IGameModeComponent
{
    public SC_IGameMode GameMode {get; private set;} = null!;
    [Export] private UIW_CombatStats _uiCombatStats = null!;
    [Export] private PI_Stats? _statsInput;

	[Signal] public delegate void EarnScoreEventHandler(uint earned, uint score);
	public Observable<uint> Score {get; private set;} = new();
	public event Action<Guid, int>? ScoreSubmitted;

	public STAT_ArenaGame GameStats {get; private set;} = null!;

    private ulong _startTime;

    public override void _Ready()
    {
        if (!SC_IGameModeComponent.RetrieveGameMode(this, out SC_IGameMode? gameMode))
            return;

        GameMode = gameMode;
        
		_statsInput?.DisableAction();

        GameMode.EnemyKilled += OnEnemyKilled;
        GameMode.Initialized += OnInitialized;
        GameMode.Started += OnStart;
        GameMode.Interrupted += OnInterrupted;
    }

    private void StartTimer() =>
        _startTime = PHX_Time.ScaledTicksMsec;

    private void StopTimer() =>
        GameStats.Time = PHX_Time.ScaledTicksMsec - _startTime;

    private void OnInitialized() => Init(null!);
    private void OnStart() => Start();
    private void OnInterrupted(GameModeEnd outcome) => Interrupt(outcome);

    private async void SendScore(SubmitScoreRequest scoreReq)
	{
		ScoreResult result = await ApiGodotGlue.Instance.SubmitScore(scoreReq);

		if (result.Success && result.ScoreId != null && result.Rank != null)
			ScoreSubmitted?.Invoke((Guid)result.ScoreId, (int)result.Rank);
	}

    private void InitNewPlayer(GE_IActiveCombatEntity player)
	{
		_uiCombatStats.Clear();
		GameStats = new(new(player));
		_uiCombatStats.AddStat(GameStats.CombatStat, Score);
	}

    // TYPE DEPENDANT -- before we make it generic
    private void OnEnemyKilled(E_IEnemy enemy, GC_Health senderLayer)
	{
		GameStats.Score += enemy.Score;
        Score.Value = GameStats.Score;
		EmitSignal(SignalName.EarnScore, enemy.Score, Score.Value);
	}

    public bool Init(GE_IActiveCombatEntity starter)
    {
        if (GameMode.Player is null)
            return false;

        InitNewPlayer(GameMode.Player);
        return true;
    }

    public bool Start()
    {
        _statsInput?.EnableAction();
        StartTimer();
        return true;
    }

    public bool Interrupt(GameModeEnd outcome)
    {
        StopTimer();
		_statsInput?.DisableAction();

        // TYPE DEPENDANT -- before we make it generic
        if (outcome == GameModeEnd.Surrender)
            return false;

        if (!Session.IsAuthenticated)
            return false;

        SendScore(GameStats.ToScoreReq());
        return true;
    }
}