using System;
using Godot;

[GlobalClass]
public partial class SC_RandomParentSector : SC_ParentSector
{
    [Export] private int _count;
    private int _subSectorIndex;
    private readonly Random _rng = new();

    private int _elapsedCount;

    protected override bool OnSectorHandleNextSpec(SC_GenericSpawnerScript prev)
    {
        _elapsedCount ++;
        if (_elapsedCount >= _count)
        {
            DoStop();
            return false;
        }

        _subSectorIndex = GetRandomIndex();
        ActiveSector = _subSectors[_subSectorIndex];
        ActiveSector.Start();

        return true;
    }

    /// <summary>
    /// Returns the index of the next sector, excluding the current.
    /// </summary>
    /// <returns></returns>
    private int GetRandomIndex() =>
        (_rng.Next(_subSectors.Count - 1) + 1 + _subSectorIndex) % _subSectors.Count;

    protected override void ParentReadySpec() {}

    protected override bool StartSpec()
    {
        _elapsedCount = 0;
        ActiveSector = _subSectors[_subSectorIndex];
        ActiveSector.Start();

        return true;
    }

    protected override bool InterruptSpec(GameModeEnd outcome)
    {
        ActiveSector?.Interrupt(outcome);
        return true;
    }

    protected override void ParentInitSpec(GE_IActiveCombatEntity starter)
    {
        _subSectorIndex = _rng.Next(_subSectors.Count);
        ActiveSector = _subSectors[_subSectorIndex];
    }
}