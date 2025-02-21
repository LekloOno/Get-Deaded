using Godot;

[GlobalClass]
public partial class PA_Dash : PA_LayeredSound
{
    [Export] private PM_Dash _dash;

    public override void _Ready()
    {
        _dash.OnStart += (o, e) => PlayLayers();
    }
}