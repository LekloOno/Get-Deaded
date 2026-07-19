using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class SC_ParentSector : SC_SpawnSector
{
    protected readonly List<SC_SpawnSector> _subSectors = [];
    public SC_SpawnSector? ActiveSector {get; protected set;}

    [Signal] public delegate void SectorChangedToEventHandler(SC_LeafSector sector);
    [Signal] public delegate void SectorChangedEventHandler();

    protected override sealed void ReadySpec()
    {
        _subSectors.Clear();
        foreach (Node node in GetChildren())
            if (node is SC_SpawnSector sector)
            {
                _subSectors.Add(sector);
                sector.HandlingPassed += OnSectorHandleNext;
                if (sector is SC_ParentSector parent)
                    parent.SectorChangedTo += EmitSectorChanged;
            }

        ParentReadySpec();
    }

    private void EmitSectorChanged(SC_LeafSector sector)
    {
        EmitSignal(SignalName.SectorChangedTo, sector);
        EmitSignal(SignalName.SectorChanged);
    }

    protected override void InitSpec(GE_IActiveCombatEntity starter)
    {
        foreach (SC_SpawnSector subSector in _subSectors)
            subSector.Init(starter);
        ParentInitSpec(starter);
    }

    public override SC_LeafSector? ActiveLeafSector() =>
        ActiveSector?.ActiveLeafSector();

    protected void OnSectorHandleNext(SC_GenericSpawnerScript prev)
    {
        if (OnSectorHandleNextSpec(prev))
            EmitSectorChanged(ActiveLeafSector()!);
    }

    protected abstract void ParentInitSpec(GE_IActiveCombatEntity starter);

    protected abstract bool OnSectorHandleNextSpec(SC_GenericSpawnerScript prev);
    protected abstract void ParentReadySpec();

    protected override void OnEnemyDiedSpec(E_IEnemy enemy, GC_Health? senderLayer) {}
    protected override void OnEnemyPooledSpec(E_IEnemy enemy) {}
    protected override void OnEnemyCleared(E_IEnemy enemy) {}
    protected override void OnEnemyDisabledSpec(E_IEnemy enemy) {}
}