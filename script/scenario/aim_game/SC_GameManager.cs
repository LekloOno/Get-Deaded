using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class SC_GameManager : Node
{
    [Export] private SC_SpawnerScript _initial;
    [Export] private float _countDown = 2f;
    [Export] private float _killRegen = 10f;
    [Signal] public delegate void InitializeEventHandler(SC_GameManager manager);
    [Signal] public delegate void ResetGameEventHandler();
    public STAT_Combat Stats {get; private set;}
    private uint _score;
    public SceneTreeTimer CountDownTimer;
    private GE_IActiveCombatEntity _player;
    private GC_Shield _shield;
    /// <summary>
    /// An "unexpected" stop, like the player manually stopping, or dying.
    /// </summary>
    public Action Interrupt;

    public void Init(GE_IActiveCombatEntity player)
    {
        if (_player != null)
            _player.HealthManager.OnDie -= DoInterrupt;

        _player = player;

        foreach (PW_Weapon weapon in player.WeaponsHandler.Weapons)
                foreach (PW_Fire fire in weapon.Fires)
                    fire.Ammos.Initialize();
                    
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
        
        if (_player.HealthManager.TopHealthLayer is GC_Shield shield)
            _shield = shield;

        _player?.WeaponsHandler.EnableFire();
        _initial.Start(_player);
        SC_EntitiesManager.EnablePickups();
    }

    public void HandleKill(E_IEnemy enemy, GC_Health senderLayer)
    {
        _score += enemy.Score;

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
        SC_EntitiesManager.DisablePickups();
        Stats.Disable();
        EmitSignal(SignalName.ResetGame);
    }
}