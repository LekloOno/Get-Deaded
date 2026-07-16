using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class SC_ParentSector : SC_SpawnSector
{
    protected readonly List<SC_SpawnSector> _subSectors = [];
    public SC_SpawnSector? ActiveSector {get; protected set;}

    public event Action<SC_LeafSector>? SectorChanged;

    protected override sealed void ReadySpec()
    {
        _subSectors.Clear();
        foreach (Node node in GetChildren())
            if (node is SC_SpawnSector sector)
            {
                _subSectors.Add(sector);
                sector.HandleNext += OnSectorHandleNext;
                if (sector is SC_ParentSector parent)
                    parent.SectorChanged += (s) => SectorChanged?.Invoke(s);
            }

        ParentReadySpec();
    }

    protected override void InitSpec()
    {
        foreach (SC_SpawnSector subSector in _subSectors)
            subSector.Init();
        ParentInitSpec();
    }

    public override SC_LeafSector? ActiveLeafSector() =>
        ActiveSector?.ActiveLeafSector();

    protected void OnSectorHandleNext(SC_GenericSpawnerScript prev)
    {
        if (OnSectorHandleNextSpec(prev))
            SectorChanged?.Invoke(ActiveLeafSector()!);
    }

    protected abstract void ParentInitSpec();

    protected abstract bool OnSectorHandleNextSpec(SC_GenericSpawnerScript prev);
    protected abstract void ParentReadySpec();

    protected override void OnEnemyDiedSpec(E_IEnemy enemy, GC_Health? senderLayer) {}
    protected override void OnEnemyPooledSpec(E_IEnemy enemy) {}
    protected override void OnEnemyCleared(E_IEnemy enemy) {}
    protected override void OnEnemyDisabledSpec(E_IEnemy enemy) {}
}