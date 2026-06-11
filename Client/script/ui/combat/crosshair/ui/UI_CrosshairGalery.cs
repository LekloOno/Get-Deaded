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
            UI_CrosshairPreviewContainer preview = _crosshairStaticPreview.Instantiate<UI_CrosshairPreviewContainer>();
            preview.Init(crosshair);
            preview.Selected += (_) => _menu.ExitCurrent();
            _container.AddChild(preview);  
        }
    }
}