using System;
using Godot;

[GlobalClass]
public partial class SC_RandomParentSector : SC_ParentSector
{
    [Export] private int _count;
    private int _subSectorIndex;
    private readonly Random _rng = new();

    private int _elapsedCount;

    protected override void OnSectorHandleNext(SC_GenericSpawnerScript prev)
    {
        _elapsedCount ++;
        if (_elapsedCount >= _count)
        {
            DoStop();
            return;
        }

        _subSectorIndex = GetRandomIndex();
        SC_SpawnSector sector = _subSectors[_subSectorIndex];
        sector.Start(prev.Starter);
    }

    /// <summary>
    /// Returns the index of the next sector, excluding the current.
    /// </summary>
    /// <returns></returns>
    private int GetRandomIndex() =>
        (_rng.Next(_subSectors.Count - 1) + 1 + _subSectorIndex) % _subSectors.Count;

    protected override void ParentReadySpec() {}

    protected override void StartSpec(GE_IActiveCombatEntity starter)
    {
        _subSectorIndex = _rng.Next(_subSectors.Count);
        _elapsedCount = 0;
        SC_SpawnSector sector = _subSectors[_subSectorIndex];
        sector.Start(Starter);
    }

    protected override void StopSpec()
    {
        _subSectors[_subSectorIndex].Interrupt();
    }
}