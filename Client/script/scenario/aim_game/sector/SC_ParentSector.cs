using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class SC_ParentSector : SC_SpawnSector
{
    protected readonly List<SC_SpawnSector> _subSectors = [];

    protected override sealed void ReadySpec()
    {
        foreach (Node node in GetChildren())
            if (node is SC_SpawnSector sector)
            {
                _subSectors.Add(sector);
                sector.HandleNext += OnSectorHandleNext;
            }

        ParentReadySpec();
    }

    protected abstract void OnSectorHandleNext(SC_GenericSpawnerScript prev);
    protected abstract void ParentReadySpec();

    protected override void OnEnemyDiedSpec(E_IEnemy enemy, GC_Health? senderLayer) {}
    protected override void OnEnemyPooledSpec(E_IEnemy enemy) {}
    protected override void OnEnemyCleared(E_IEnemy enemy) {}
    protected override void OnEnemyDisabledSpec(E_IEnemy enemy) {}
}