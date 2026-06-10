using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_CrosshairGalery : Container
{
    [Export] private PackedScene _crosshairStaticPreview = null!;

    public void Init(List<CrosshairData> crosshairs)
    {
        foreach (Node node in GetChildren())
            node.QueueFree();

        foreach (CrosshairData crosshair in crosshairs)
        {
            CrosshairPreview preview = _crosshairStaticPreview.Instantiate<CrosshairPreview>();
            preview.Data = crosshair;
            AddChild(preview);  
        }
    }
}