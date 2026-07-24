using System;
using Godot;

[GlobalClass]
public partial class SC_RandomParentSector : SC_ParentSector
{
    [Export] private int _count;
    [Export] private bool _pickUnique = true;
    private int _subSectorIndex;
    private readonly Random _rng = new();

    private RandomStash<SC_SpawnSector> _remainingSubSectors = null!;

    private int _elapsedCount;

    protected override bool OnSectorHandleNextSpec(SC_GenericSpawnerScript prev)
    {
        _elapsedCount ++;
        if (_elapsedCount >= _count)
        {
            PendingNext = null; // explicit, but should be true anyways
            DoStop();
            return false;
        }

        ActiveSector = PendingNext!;
        ActiveSector.Start();

        PreparePendingNext();

        return true;
    }

    private void PreparePendingNext()
    {
        PendingNext = (_elapsedCount + 1 >= _count) ? null : PickNext();
    }

    private SC_SpawnSector PickNext()
    {
        if (_pickUnique)
            return _remainingSubSectors.Draw();

        _subSectorIndex = GetRandomIndex();
        return _subSectors[_subSectorIndex];
    }

    /// <summary>
    /// Returns the index of the next sector, excluding the current.
    /// </summary>
    /// <returns></returns>
    private int GetRandomIndex() =>
        (_rng.Next(_subSectors.Count - 1) + 1 + _subSectorIndex) % _subSectors.Count;

    protected override void ParentReadySpec()
    {
        if (_pickUnique)
            _remainingSubSectors = new(_subSectors, true, _rng);
    }

    protected override bool StartSpec()
    {
        _elapsedCount = 0;
        ActiveSector!.Start();

        if (PendingNext == null)
            PreparePendingNext();

        return true;
    }

    protected override bool InterruptSpec(GameModeEnd outcome)
    {
        ActiveSector?.Interrupt(outcome);
        PendingNext = null;
        return true;
    }

    protected override void ParentInitSpec(GE_IActiveCombatEntity starter)
    {
        if (_pickUnique)
        {
            _remainingSubSectors.Reset();
            ActiveSector = _remainingSubSectors.Draw();
        }
        else
        {
            _subSectorIndex = _rng.Next(_subSectors.Count);
            ActiveSector = _subSectors[_subSectorIndex];
        }

        PreparePendingNext();
    }
}