using Godot;

[GlobalClass]
public partial class PA_LedgeClimb : Node3D
{
    [Export] private AUD_Sound _sound;
    [Export] private PM_LedgeClimb _LedgeClimb;
    public override void _Ready()
    {
        _LedgeClimb.OnStart += (o, e) => _sound.Play();
    }
}