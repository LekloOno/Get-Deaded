using System;
using Godot;

[GlobalClass]
public partial class SC_GameManager : Node
{
    [Export] private SC_SpawnerScript _initial;
    [Export] private float _countDown = 2f;
    [Export] private float _killRegen = 10f;
    [Signal] public delegate void InitializeEventHandler(SC_GameManager manager);
    [Signal] public delegate void StartGameEventHandler();
    [Signal] public delegate void ResetGameEventHandler();
    [Signal] public delegate void EarnScoreEventHandler(uint earned, uint score);
    [Export] private UIW_CombatStats _combatStats;
    [Export] private PI_Stats _statsInput;
    public STAT_Combat Stats {get; private set;}
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
            Stats.Reset();

        foreach (PW_Weapon weapon in player.WeaponsHandler.Weapons)
                foreach (PW_Fire fire in weapon.Fires)
                    fire.Ammos.Initialize();

        Score.Value = 0;

        player.WeaponsHandler.DisableFire();

        EmitSignal(SignalName.Initialize, this);

        CountDownTimer = GetTree().CreateTimer(_countDown, false, true);
        CountDownTimer.Timeout += Start;
    }

    private void InitNewPlayer(GE_IActiveCombatEntity player)
    {
        if (_player != null)
            _player.HealthManager.OnDie -= DoInterrupt;

        _combatStats.Clear();
        Stats = new(player);
        _combatStats.AddStat(Stats, Score);
                    
        player.HealthManager.OnDie += DoInterrupt;
        _player = player;
    }

    private void DoInterrupt(GC_Health _)
    {
        Interrupt?.Invoke();
        Reset();
    }

    private void Start()
    {
        CountDownTimer.Timeout -= Start;

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

    private void Reset()
    {
        _statsInput.DisableAction();
        SC_EntitiesManager.DisablePickups();
        EmitSignal(SignalName.ResetGame);
    }
}