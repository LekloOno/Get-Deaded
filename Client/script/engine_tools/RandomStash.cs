using System;
using System.Collections.Generic;

public sealed class RandomStash<T>
{
    private readonly List<T> _source;
    private readonly List<T> _stash;
    private readonly Random _random;

    private readonly bool _avoidBoundaryRepeat;
    private bool _pendingExclusion;
    private int _excludedIndex = -1;

    private T? _lastDrawn;
    private bool _hasLastDrawn;

    public int RemainingInCycle => _stash.Count;
    public int SourceCount => _source.Count;

    public RandomStash(IEnumerable<T> source, bool avoidBoundaryRepeat = true, Random? random = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        _source = [.. source];

        if (_source.Count == 0)
            throw new ArgumentException("RandomStash source must contain at least one element.", nameof(source));

        _random = random ?? Random.Shared;
        _avoidBoundaryRepeat = avoidBoundaryRepeat;
        _stash = new List<T>(_source.Count);

        Refill();
    }

    public T Draw()
    {
        if (_stash.Count == 0)
            Refill();

        int index;
        if (_pendingExclusion && _stash.Count > 1)
        {
            int r = _random.Next(_stash.Count - 1);
            index = (r + 1 + _excludedIndex) % _stash.Count;
        }
        else
        {
            index = _random.Next(_stash.Count);
        }
        _pendingExclusion = false;

        T item = _stash[index];
        _stash[index] = _stash[^1];
        _stash.RemoveAt(_stash.Count - 1);

        if (_avoidBoundaryRepeat && _stash.Count == 0)
        {
            _excludedIndex = _source.IndexOf(item);
            _pendingExclusion = true;
        }

        return item;
    }

    public void Reset()
    {
        _pendingExclusion = false;
        Refill();
    }

    private void Refill()
    {
        _stash.Clear();
        _stash.AddRange(_source);
    }
}