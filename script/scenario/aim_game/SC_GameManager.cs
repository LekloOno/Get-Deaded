using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class SC_GameManager : Node
{
    [Export] private SC_SpawnerScript _initial;
    [Export] private float _countDown = 2f;
    [Export] private float _killRegen = 10f;
    [Signal] public delegate void InitializeEventHandler(SC_GameManager manager);
    public STAT_Combat Stats {get; private set;}
    private uint _score;
    public SceneTreeTimer CountDownTimer;
    private GE_IActiveCombatEntity _player;
    private GC_SpeedShield _speedShield;
    /// <summary>
    /// An "unexpected" stop, like the player manually stopping, or dying.
    /// </summary>
    public Action Interrupt;

    public void Init(GE_IActiveCombatEntity player)
    {
        if (_player != null)
            _player.HealthManager.OnDie -= DoInterrupt;

        _player = player;
        _player.HealthManager.OnDie += DoInterrupt;

        Stats = new(player);
        _score = 0;

        player?.WeaponsHandler.DisableFire();

        EmitSignal(SignalName.Initialize, this);

        CountDownTimer = GetTree().CreateTimer(_countDown, false, true);
        CountDownTimer.Timeout += StartGame;
    }

    private void DoInterrupt(GC_Health _)
    {
        Interrupt?.Invoke();
        Reset();
    }

    private void StartGame()
    {
        CountDownTimer.Timeout -= StartGame;
        
        if (_player.HealthManager.TopHealthLayer is GC_SpeedShield speedShield)
        {
            _speedShield = speedShield;
            _speedShield.Active = true;
        }

        _player?.WeaponsHandler.EnableFire();
        _initial.Start(_player);
    }

    public void HandleKill(E_IEnemy enemy, GC_Health senderLayer)
    {
        _score += enemy.Score;
        _speedShield?.Regen(_killRegen);
    }

    public void EndGame()
    {
        _player.HealthManager.Heal(99999);
        Reset();
    }

    private void Reset()
    {
        Stats.Disable();
        if (_speedShield != null)
            _speedShield.Active = false;
    }
}