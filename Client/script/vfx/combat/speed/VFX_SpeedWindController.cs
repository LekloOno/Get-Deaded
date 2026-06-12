using System.Collections.Generic;
using Godot;

[GlobalClass, Tool]
public partial class VFX_SpeedWindController : Control
{
    [Export(PropertyHint.Range, "0,1")]
    public float Intensity;

    private readonly List<VFX_SpeedWindLayer> _layers = [];

    public override void _EnterTree()
    {
        SetChildren();
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        foreach (VFX_SpeedWindLayer layer in _layers)
            layer.Intensity = Intensity;
    }

    public override void _Notification(int what)
    {
        if (what != NotificationChildOrderChanged)
            return;
        
        SetChildren();
    }

    private void SetChildren()
    {
        _layers.Clear();
        foreach(Node node in GetChildren())
            if (node is VFX_SpeedWindLayer layer)
                _layers.Add(layer);
    }
}