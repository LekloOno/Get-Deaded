using System;
using Godot;

[GlobalClass]
public partial class GcPressure : Node
{
    private int _lastGen2 = 0;
    public override void _Process(double delta)
    {
        int gen2 = GC.CollectionCount(2);
        if (gen2 != _lastGen2)
        {
            GD.Print($"[GC] Gen2 collection #{gen2} at t={Time.GetTicksMsec()}ms");
            _lastGen2 = gen2;
        }
    }
}