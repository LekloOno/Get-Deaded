using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_CrosshairGalery : Control
{
    [Export] private PackedScene _crosshairStaticPreview = null!;
    [Export] private Container   _galery = null!;

    public void Init(List<CrosshairData> crosshairs)
    {
        foreach (Node node in _galery.GetChildren())
            node.QueueFree();

        foreach (CrosshairData crosshair in crosshairs)
        {
            CrosshairPreview preview = _crosshairStaticPreview.Instantiate<CrosshairPreview>();
            preview.Data = crosshair;
            _galery.AddChild(preview);  
        }
    }
}