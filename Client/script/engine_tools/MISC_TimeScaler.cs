using Godot;

[GlobalClass]
public partial class MISC_TimeScaler : Node
{
    private bool _slowMo = false;
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_slowMo)
            return;
            
        if (@event is InputEventMouseButton)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
                    Engine.TimeScale *= 1.1;

                if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
                    Engine.TimeScale /= 1.1;
            }
        }
    }
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("slow_mo"))
            return;

        if (_slowMo)
            Engine.TimeScale = 1;
        
        _slowMo = !_slowMo;
    }
}