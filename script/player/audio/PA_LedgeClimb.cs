using Godot;

[GlobalClass]
public partial class PA_LedgeClimb : PA_LayeredSound
{
    [Export] private PM_LedgeClimb _LedgeClimb;
    public override void _Ready()
    {
        _LedgeClimb.OnStart += (o, e) => PlayLayers();
    }
}