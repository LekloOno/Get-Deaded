using Godot;

[GlobalClass]
public partial class CrosshairPreview : CrosshairRenderer
{
    public override void _Ready(){}

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
            if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
                CrosshairSetting.Instance.Save(Data); 
    }
}