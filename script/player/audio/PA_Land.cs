using GaudioProcessTree;
using Godot;

[GlobalClass]
public partial class PA_Land : Node3D
{
    [Export] private AUD_Sound _sound;
    [Export] private PS_Grounded _groundState;

    public override void _Ready()
    {
        _groundState.OnLanding += (o, e) => _sound.Play();
    }
}