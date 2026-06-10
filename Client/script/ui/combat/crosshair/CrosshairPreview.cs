using System;
using System.Diagnostics.Tracing;
using Godot;

[GlobalClass]
public partial class CrosshairPreview : CrosshairRenderer
{
    public event Action<CrosshairData>? Selected;
    public override void _Ready() {}

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mb)
            return;

        if (mb.ButtonIndex != MouseButton.Left || !mb.Pressed)
            return;

        CrosshairSetting.Instance.Save(Data);
        Selected?.Invoke(Data); 
    }
}