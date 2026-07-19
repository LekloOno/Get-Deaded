using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class SC_LeafSector : SC_SpawnSector
{
    protected readonly List<Node3D> _spawnPoints = [];

    protected override sealed void ReadySpec()
    {
        foreach (Node node in GetChildren())
            if (node is Node3D point)
                _spawnPoints.Add(point);

        LeafReadySpec();
    }

    public override SC_LeafSector? ActiveLeafSector() => this;

    protected abstract void LeafReadySpec();
}