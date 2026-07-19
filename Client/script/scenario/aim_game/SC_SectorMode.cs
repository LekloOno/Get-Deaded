using Godot;

public partial class SC_SectorMode : SC_GameMode
{
    [Export] private SC_ParentSector _root = null!;

    protected override void ReadySpec() {}

    public void DoStart() => Start();

    protected override bool InitSpec(GE_IActiveCombatEntity starter)
    {
        _root.Init(starter);
        return true;
    }

    protected override bool StartSpec()
    {
        SC_EntitiesManager.EnablePickups();
        _root.Start();    

        return true;
    }

    protected override bool InterruptSpec(GameModeEnd outcome)
    {
        _root.Interrupt(outcome);
        SC_EntitiesManager.DisablePickups();

        return true;
    }

    protected override void OnPlayerDeath(GC_Health senderLayer)
    {
        Interrupt(GameModeEnd.Lost);
    }
}