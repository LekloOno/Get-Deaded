using Godot;

namespace Pew;

[GlobalClass]
public partial class PA_Land : PA_LayeredSound
{
    [Export] private PS_Grounded _groundState;

    public override void _Ready()
    {
        _groundState.OnLanding += (o, e) => PlayLayers();
    }
}