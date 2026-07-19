using Godot;

public partial class SC_SectorMode : SC_GameMode
{
    [Export] private SC_ParentSector _root = null!;
    [Export] private float _runTime = 160;
    public SceneTreeTimer? _runTimer;

    protected override void ReadySpec() {}

    public void DoStart() => Start();

    protected override bool InitSpec(GE_IActiveCombatEntity starter)
    {
        _root.Init(starter);
        return true;
    }

    protected override bool StartSpec()
    {
        if (_runTimer != null)
            return false;

        _runTimer = GetTree().CreateTimer(_runTime, false, true);
        _runTimer.Timeout += OnRunTimeout;

        SC_EntitiesManager.EnablePickups();
        _root.Start();    

        return true;
    }

    private void OnRunTimeout() =>
        Interrupt(GameModeEnd.Win);

    protected override bool InterruptSpec(GameModeEnd outcome)
    {        
        //if (_runTimer == null)
        //    return false;

        if (_runTimer != null)
        {
            _runTimer.Timeout -= OnRunTimeout;
            _runTimer = null;    
        }

        _root.Interrupt(outcome);
        SC_EntitiesManager.DisablePickups();

        return true;
    }

    protected override void OnPlayerDeath(GC_Health senderLayer)
    {
        Interrupt(GameModeEnd.Lost);
    }
}