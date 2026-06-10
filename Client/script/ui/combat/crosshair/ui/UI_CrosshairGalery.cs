using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_CrosshairGalery : Control
{
    [Export] private UI_EscapeMenu _menu = null!;
    [Export] private Container     _container = null!;
    [Export] private PackedScene   _crosshairStaticPreview = null!;

    public void Init(List<CrosshairData> crosshairs)
    {
        foreach (Node node in _container.GetChildren())
            node.QueueFree();

        foreach (CrosshairData crosshair in crosshairs)
        {
            CrosshairPreview preview = _crosshairStaticPreview.Instantiate<CrosshairPreview>();
            preview.Data = crosshair;
            preview.Selected += (_) => _menu.ExitCurrent();
            _container.AddChild(preview);  
        }
    }
}